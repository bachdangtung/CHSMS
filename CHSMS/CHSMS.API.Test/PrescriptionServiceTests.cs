using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace CHSMS.API.Tests.Services
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();

            var options = new DbContextOptionsBuilder<SEP_TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new SEP_TestContext(options);
            _service = new PrescriptionService(_repositoryMock.Object, _dbContext);
        }

        #region CreatePrescriptionAsync
        [Fact]
        public async Task CreatePrescriptionAsync_ValidData_ReturnsNewPrescriptionId()
        {
            var dto = GetValidDTO();

            SetupInventory(1, 10, 20, DateTime.Now.AddDays(30));

            _repositoryMock.Setup(r => r.CreatePrescriptionAsync(It.IsAny<Prescription>()))
                .ReturnsAsync((Prescription p) => { p.PrescriptionId = 1; return p; });

            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                .ReturnsAsync((MedicineConsumption m) => { m.MedicineConsumptionId = 1; return m; });

            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                .ReturnsAsync((PrescriptionMedicineConsumption pmc) => pmc);

            var result = await _service.CreatePrescriptionAsync(1, 1, dto);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_FutureIssueDate_ThrowsException()
        {
            var dto = GetValidDTO();
            dto.IssueDate = DateTime.Now.AddDays(1);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("ngày trong tương lai", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_TooManyMedicines_ThrowsException()
        {
            var dto = GetValidDTO();
            dto.MedicineConsumptions = Enumerable.Range(1, 11).Select(i =>
                new MedicineConsumptionDTO
                {
                    MedicineInventoryId = i,
                    Amount = 1,
                    ConsumptionDate = DateTime.Now
                }).ToList();

            foreach (var id in dto.MedicineConsumptions.Select(x => x.MedicineInventoryId))
            {
                SetupInventory(id, id, 20, DateTime.Now.AddDays(30));
            }

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("quá 10 loại thuốc", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_DuplicateMedicines_ThrowsException()
        {
            var dto = GetValidDTO();
            dto.MedicineConsumptions.Add(dto.MedicineConsumptions[0]); // duplicate

            SetupInventory(1, 10, 20, DateTime.Now.AddDays(30));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("bị trùng", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_InsufficientInventory_ThrowsException()
        {
            var dto = GetValidDTO();
            dto.MedicineConsumptions[0].Amount = 50;

            SetupInventory(1, 10, 20, DateTime.Now.AddDays(30));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("vượt quá tồn kho", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_ExpiredMedicine_ThrowsException()
        {
            var dto = GetValidDTO();

            SetupInventory(1, 10, 20, DateTime.Now.AddDays(-1)); // expired

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("vượt quá hạn sử dụng", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_BelowMinimumQuantity_ThrowsException()
        {
            var dto = GetValidDTO();
            dto.MedicineConsumptions[0].Amount = 15;

            SetupInventory(1, 10, 20, DateTime.Now.AddDays(30)); // 20 - 15 < 10

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("dưới ngưỡng tối thiểu", ex.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_MissingInventory_ThrowsException()
        {
            var dto = GetValidDTO();

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MedicineInventory)null); // not found

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Không tìm thấy kho thuốc", ex.Message);
        }

        #endregion

        #region EditPrescriptionForDoctor

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();
            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            SetupInventory(2, 20, 30, DateTime.Now.AddDays(30)); // For the new medicine

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                .ReturnsAsync((MedicineConsumption m) => { m.MedicineConsumptionId = 2; return m; });
            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                .ReturnsAsync((PrescriptionMedicineConsumption pmc) => pmc);
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new PrescriptionMedicineConsumption { PrescriptionId = 1, MedicineConsumtionId = 3 });
            _repositoryMock.Setup(r => r.DeletePrescriptionMedicineConsumptionAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteMedicineConsumptionAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()), Times.Once);
            _repositoryMock.Verify(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()), Times.Once);
            _repositoryMock.Verify(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()), Times.Once);
            _repositoryMock.Verify(r => r.DeleteMedicineConsumptionAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();
            dto.IssueDate = DateTime.Now.AddDays(1); // Future date

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("ngày trong tương lai", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_PrescriptionNotFound_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync((Prescription)null); // Prescription not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Không tìm thấy đơn thuốc", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_PrescriptionCompleted_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();
            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = true } // All consumptions are completed
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("đã được xác nhận hoàn tất", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_TooManyMedicines_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();

            // Add 10 consumptions to the DTO
            dto.MedicineConsumptionsToAdd = Enumerable.Range(1, 10).Select(i =>
                new MedicineConsumptionDTO
                {
                    MedicineInventoryId = i + 1,
                    Amount = 1,
                    ConsumptionDate = DateTime.Now,
                    IsSpecialMedicine = false,
                    Note = "Test"
                }).ToList();

            var prescription = GetTestPrescription();
            var existingConsumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false } // Already has 1 consumption
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(existingConsumptions);

            // For each inventory setup
            foreach (var dto_item in dto.MedicineConsumptionsToAdd)
            {
                SetupInventory(dto_item.MedicineInventoryId, dto_item.MedicineInventoryId, 30, DateTime.Now.AddDays(30));
            }

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("không được chứa quá 10 loại thuốc", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_DuplicateMedicines_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();

            // Add duplicate medicine inventory IDs
            dto.MedicineConsumptionsToAdd.Add(new MedicineConsumptionDTO
            {
                MedicineInventoryId = 2, // Same as the one in GetValidEditPrescriptionDTO
                Amount = 1,
                ConsumptionDate = DateTime.Now,
                IsSpecialMedicine = false,
                Note = "Test"
            });

            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("bị trùng trong danh sách", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_InsufficientInventory_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();
            dto.MedicineConsumptionsToAdd[0].Amount = 50; // Requesting more than available

            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            SetupInventory(2, 20, 30, DateTime.Now.AddDays(30)); // Only 30 available

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("vượt quá tồn kho", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_ExpiredMedicine_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();

            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            SetupInventory(2, 20, 30, DateTime.Now.AddDays(-1)); // Expired medicine

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("vượt quá hạn sử dụng", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_BelowMinimumQuantity_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();
            dto.MedicineConsumptionsToAdd[0].Amount = 25; // Will drop inventory below minimum (30-25 < 10)

            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            SetupInventory(2, 20, 30, DateTime.Now.AddDays(30)); // 30 available, minimum is 10

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("dưới ngưỡng tối thiểu", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_MissingInventory_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionDTO();

            var prescription = GetTestPrescription();
            var consumptions = new List<MedicineConsumption>
    {
        new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                .ReturnsAsync(consumptions);

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MedicineInventory)null); // Inventory not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Không tìm thấy kho thuốc", ex.Message);
        }

        #endregion

        #region EditPrescriptionForPharmacistAsync

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_ValidDispenseMedicine_UpdatesSuccessfully()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true); // Dispense medicine (Status = true)
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now;

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false, // Not dispensed yet
                MedicineInventoryId = 1,
                Amount = 5
            };

            var inventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 20,
                Medicine = new Medicine { SellingPrice = 10 }
            };

            var pmc = new PrescriptionMedicineConsumption
            {
                PrescriptionId = 1,
                MedicineConsumtionId = 1,
                TotalPrice = 0
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync(inventory);
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                .ReturnsAsync(pmc);
            _repositoryMock.Setup(r => r.UpdateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicineInventoryAsync(It.IsAny<MedicineInventory>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateMedicineConsumptionAsync(It.Is<MedicineConsumption>(m =>
                m.Status == true)), Times.Once);
            _repositoryMock.Verify(r => r.UpdateMedicineInventoryAsync(It.Is<MedicineInventory>(i =>
                i.Quantity == 15)), Times.Once); // 20 - 5 = 15
            _repositoryMock.Verify(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.Is<PrescriptionMedicineConsumption>(p =>
                p.TotalPrice == 50)), Times.Once); // 5 * 10 = 50
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p =>
                p.Status == true)), Times.Once);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_ValidRollback_UpdatesSuccessfully()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(false); // Rollback (Status = false)
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now;

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = true, // Previously dispensed
                MedicineInventoryId = 1,
                Amount = 5
            };

            var inventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 15,
                Medicine = new Medicine { SellingPrice = 10 }
            };

            var pmc = new PrescriptionMedicineConsumption
            {
                PrescriptionId = 1,
                MedicineConsumtionId = 1,
                TotalPrice = 50
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync(inventory);
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                .ReturnsAsync(pmc);
            _repositoryMock.Setup(r => r.UpdateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicineInventoryAsync(It.IsAny<MedicineInventory>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateMedicineConsumptionAsync(It.Is<MedicineConsumption>(m =>
                m.Status == false)), Times.Once);
            _repositoryMock.Verify(r => r.UpdateMedicineInventoryAsync(It.Is<MedicineInventory>(i =>
                i.Quantity == 20)), Times.Once); // 15 + 5 = 20
            _repositoryMock.Verify(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.Is<PrescriptionMedicineConsumption>(p =>
                p.TotalPrice == 0)), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p =>
                p.Status == false)), Times.Once);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_PrescriptionNotFound_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync((Prescription)null); // Prescription not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Không tìm thấy đơn thuốc", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_NotSameDay_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now.AddDays(-1); // Not today

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Chỉ được chỉnh sửa trạng thái đơn thuốc trong ngày phát hành", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_ConsumptionNotFound_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync((MedicineConsumption)null); // Consumption not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Không tìm thấy MedicineConsumption", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_InvalidRollback_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(false); // Attempt to rollback
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false, // Not dispensed yet, can't rollback
                MedicineInventoryId = 1,
                Amount = 5
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("đã được rollback trước đó hoặc chưa được phát thuốc", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_MedicineInventoryIdMissing_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false,
                MedicineInventoryId = 0, // Missing MedicineInventoryId
                Amount = 5
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("MedicineInventoryId không được để trống", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_InventoryNotFound_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false,
                MedicineInventoryId = 1,
                Amount = 5
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync((MedicineInventory)null); // Inventory not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Không tìm thấy kho thuốc", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_InsufficientInventory_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false,
                MedicineInventoryId = 1,
                Amount = 10
            };

            var inventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 5, // Not enough (5 < 10)
                Medicine = new Medicine { SellingPrice = 10 }
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync(inventory);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("Số lượng tồn kho không đủ", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_BelowMinimumQuantity_ThrowsException()
        {
            // Arrange
            var dto = GetValidEditPrescriptionForPharmacistDTO(true);
            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false,
                MedicineInventoryId = 1,
                Amount = 5
            };

            var inventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 14, // After dispensing: 14 - 5 = 9 (below minimum 10)
                Medicine = new Medicine { SellingPrice = 10 }
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync(inventory);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForPharmacistAsync(dto));
            Assert.Contains("dưới ngưỡng tối thiểu", ex.Message);
        }

        [Fact]
        public async Task EditPrescriptionForPharmacistAsync_MultipleConsumptions_SetsPrescriptionStatusCorrectly()
        {
            // Arrange
            var dto = new EditPrescriptionForPharmacistDTO
            {
                PrescriptionId = 1,
                MedicineConsumptionStatuses = new List<MedicineConsumptionStatusDTO>
        {
            new MedicineConsumptionStatusDTO { MedicineConsumptionId = 1, Status = true },
            new MedicineConsumptionStatusDTO { MedicineConsumptionId = 2, Status = false }
        }
            };

            var prescription = GetTestPrescription();
            prescription.IssueDate = DateTime.Now; // Today

            var consumption1 = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                Status = false,
                MedicineInventoryId = 1,
                Amount = 5
            };

            var consumption2 = new MedicineConsumption
            {
                MedicineConsumptionId = 2,
                Status = true, // Already dispensed, now rolling back
                MedicineInventoryId = 2,
                Amount = 3
            };

            var inventory1 = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 20,
                Medicine = new Medicine { SellingPrice = 10 }
            };

            var inventory2 = new MedicineInventory
            {
                MedicineInventoryId = 2,
                Quantity = 15,
                Medicine = new Medicine { SellingPrice = 8 }
            };

            var pmc1 = new PrescriptionMedicineConsumption
            {
                PrescriptionId = 1,
                MedicineConsumtionId = 1,
                TotalPrice = 0
            };

            var pmc2 = new PrescriptionMedicineConsumption
            {
                PrescriptionId = 1,
                MedicineConsumtionId = 2,
                TotalPrice = 24
            };

            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                .ReturnsAsync(prescription);

            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(1))
                .ReturnsAsync(consumption1);
            _repositoryMock.Setup(r => r.GetMedicineConsumptionByIdAsync(2))
                .ReturnsAsync(consumption2);

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                .ReturnsAsync(inventory1);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(2))
                .ReturnsAsync(inventory2);

            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                .ReturnsAsync(pmc1);
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(2))
                .ReturnsAsync(pmc2);

            _repositoryMock.Setup(r => r.UpdateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicineInventoryAsync(It.IsAny<MedicineInventory>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p =>
                p.Status == true)), Times.Once); // Should be true since at least one consumption has status true
        }

        #endregion

        #region QueryMethodTests

        [Fact]
        public async Task GetAllMedicinesInInventoryAsync_ReturnsCorrectlyMappedData()
        {
            // Arrange
            var testData = new List<MedicineInventory>
    {
        new MedicineInventory
        {
            MedicineId = 1,
            Medicine = new Medicine {
                MedicineName = "Test Medicine",
                ActiveIngredient = "Test Ingredient",
                Dosage = "10mg",
                DosageForm = "Tablet",
                IsBhyt = true
            },
            MedicineInventoryId = 10,
            Quantity = 50,
            ExpiryDate = DateTime.Now.AddMonths(6)
        },
        new MedicineInventory
        {
            MedicineId = 2,
            Medicine = new Medicine {
                MedicineName = "Another Medicine",
                ActiveIngredient = "Another Ingredient",
                Dosage = "20mg",
                DosageForm = "Syrup",
                IsBhyt = false
            },
            MedicineInventoryId = 20,
            Quantity = 30,
            ExpiryDate = DateTime.Now.AddMonths(3)
        }
    };

            _repositoryMock.Setup(r => r.GetAvailableMedicinesAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllMedicinesInInventoryAsync();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].MedicineId);
            Assert.Equal("Test Medicine", result[0].MedicineName);
            Assert.Equal("Test Ingredient", result[0].ActiveIngredient);
            Assert.Equal("10mg", result[0].Dosage);
            Assert.Equal("Tablet", result[0].DosageForm);
            Assert.Equal(10, result[0].MedicineInventoryId);
            Assert.Equal(50, result[0].Quantity);
            Assert.Equal(testData[0].ExpiryDate, result[0].ExpiryDate);
            Assert.True(result[0].IsBhyt);

            Assert.Equal(2, result[1].MedicineId);
            Assert.Equal("Another Medicine", result[1].MedicineName);
            Assert.Equal(20, result[1].MedicineInventoryId);
            Assert.Equal(30, result[1].Quantity);
            Assert.False(result[1].IsBhyt);
        }

        [Fact]
        public async Task GetPrescriptionsByUserIdListAsync_ReturnsCorrectData()
        {
            // Arrange
            int userId = 1;
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = DateTime.Now.AddDays(-1),
            Status = true,
            Note = "Test note",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Test Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = DateTime.Now,
            Status = false,
            Note = "Another note",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionsByUserIdAsync(userId))
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetPrescriptionsByUserIdListAsync(userId);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].PrescriptionId);
            Assert.Equal(testData[0].IssueDate, result[0].IssueDate);
            Assert.True(result[0].Status);
            Assert.Equal("Test note", result[0].Note);
            Assert.True(result[0].IsBhyt);
            Assert.Equal("Test Patient", result[0].PatientName);

            Assert.Equal(2, result[1].PrescriptionId);
            Assert.Equal(testData[1].IssueDate, result[1].IssueDate);
            Assert.False(result[1].Status);
            Assert.Equal("Another note", result[1].Note);
            Assert.False(result[1].IsBhyt);
            Assert.Equal("Another Patient", result[1].PatientName);
        }

        [Fact]
        public async Task GetAllPrescriptionsAsync_ReturnsOnlyBhytPrescriptions()
        {
            // Arrange
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = DateTime.Now.AddDays(-1),
            Status = true,
            Note = "Test note",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Test Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = DateTime.Now,
            Status = false,
            Note = "Another note",
            IsBhyt = false, // Should be filtered out
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 3,
            IssueDate = DateTime.Now.AddDays(-2),
            Status = true,
            Note = "BHYT note",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "BHYT Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Equal(2, result.Count); // Only 2 BHYT prescriptions
            Assert.All(result, item => Assert.True(item.IsBhyt));
            Assert.Equal(1, result[0].PrescriptionId);
            Assert.Equal(3, result[1].PrescriptionId);
        }

        [Fact]
        public async Task GetTodayPrescriptionsAsync_ReturnsOnlyTodayBhytPrescriptions()
        {
            // Arrange
            var today = DateTime.Today;
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = today,
            Status = true,
            Note = "Today BHYT",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Today Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = today,
            Status = false,
            Note = "Today non-BHYT",
            IsBhyt = false, // Should be filtered out - not BHYT
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Non-BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 3,
            IssueDate = today.AddDays(-1),
            Status = true,
            Note = "Yesterday BHYT",
            IsBhyt = true, // Should be filtered out - not today
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Yesterday Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 4,
            IssueDate = today,
            Status = true,
            Note = "Another Today BHYT",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Today Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetTodayPrescriptionsAsync();

            // Assert
            Assert.Equal(2, result.Count); // Only today BHYT prescriptions
            Assert.All(result, item => Assert.True(item.IsBhyt));
            Assert.All(result, item => Assert.Equal(today, item.IssueDate.Date));
            Assert.Equal(1, result[0].PrescriptionId);
            Assert.Equal(4, result[1].PrescriptionId);
        }

        [Fact]
        public async Task GetAllPrescriptionsNoBHYTAsync_ReturnsOnlyNonBhytPrescriptions()
        {
            // Arrange
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = DateTime.Now.AddDays(-1),
            Status = true,
            Note = "BHYT note",
            IsBhyt = true, // Should be filtered out
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = DateTime.Now,
            Status = false,
            Note = "Non-BHYT note",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Non-BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 3,
            IssueDate = DateTime.Now.AddDays(-2),
            Status = true,
            Note = "Another Non-BHYT note",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Non-BHYT Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Equal(2, result.Count); // Only non-BHYT prescriptions
            Assert.All(result, item => Assert.False(item.IsBhyt));
            Assert.Equal(2, result[0].PrescriptionId);
            Assert.Equal(3, result[1].PrescriptionId);
        }

        [Fact]
        public async Task GetTodayPrescriptionsNoBHYTAsync_ReturnsOnlyTodayNonBhytPrescriptions()
        {
            // Arrange
            var today = DateTime.Today;
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = today,
            Status = true,
            Note = "Today BHYT",
            IsBhyt = true, // Should be filtered out - is BHYT
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Today BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = today,
            Status = false,
            Note = "Today non-BHYT",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Today Non-BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 3,
            IssueDate = today.AddDays(-1),
            Status = true,
            Note = "Yesterday non-BHYT",
            IsBhyt = false, // Should be filtered out - not today
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Yesterday Non-BHYT Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 4,
            IssueDate = today,
            Status = true,
            Note = "Another Today non-BHYT",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Today Non-BHYT Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetTodayPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Equal(2, result.Count); // Only today non-BHYT prescriptions
            Assert.All(result, item => Assert.False(item.IsBhyt));
            Assert.All(result, item => Assert.Equal(today, item.IssueDate.Date));
            Assert.Equal(2, result[0].PrescriptionId);
            Assert.Equal(4, result[1].PrescriptionId);
        }

        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_ReturnsCorrectPrescriptions()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            var testData = new List<Prescription>
    {
        new Prescription
        {
            PrescriptionId = 1,
            IssueDate = DateTime.Now.AddDays(-1),
            Status = true,
            Note = "Test note",
            IsBhyt = true,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Test Patient"
                }
            }
        },
        new Prescription
        {
            PrescriptionId = 2,
            IssueDate = DateTime.Now,
            Status = false,
            Note = "Another note",
            IsBhyt = false,
            MedicalRecordHistory = new MedicalRecordHistory
            {
                MedicalRecord = new MedicalRecord
                {
                    PatientName = "Another Patient"
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].PrescriptionId);
            Assert.Equal(2, result[1].PrescriptionId);
        }

        [Fact]
        public async Task GetPrescriptionsByMedicalRecordHistoryIdAsync_ThrowsExceptionWhenNoneFound()
        {
            // Arrange
            int medicalRecordHistoryId = 1;
            _repositoryMock.Setup(r => r.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId))
                .ReturnsAsync(new List<Prescription>()); // Empty list

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId));
            Assert.Contains("Không tìm thấy đơn thuốc", ex.Message);
        }


        [Fact]
        public async Task GetPrescriptionDetailAsync_ReturnsCorrectDetails()
        {
            // Arrange
            int prescriptionId = 1;
            var prescription = new Prescription
            {
                PrescriptionId = prescriptionId,
                IssueDate = DateTime.Now,
                Status = true,
                Note = "Test prescription",
                IsBhyt = true,
                User = new User { Fullname = "Test Doctor" },
                MedicalRecordHistory = new MedicalRecordHistory
                {
                    MedicalRecord = new MedicalRecord
                    {
                        PatientName = "Test Patient",
                        Gender = "Male",
                        Dob = new DateTime(1990, 1, 1),
                        Address = "Test Address",
                        HealthInsurance = "123456789"
                    },
                    DiagnoseConclusion = "Test Diagnosis"
                }
            };

            // Mock the direct prescription detail
            _repositoryMock.Setup(r => r.GetPrescriptionDetailAsync(prescriptionId))
                .ReturnsAsync(prescription);

            // Mock the medicine consumptions
            var prescriptionMedicineConsumptions = new List<PrescriptionMedicineConsumption>
            {
                new PrescriptionMedicineConsumption
                {
                    PrescriptionId = prescriptionId,
                    MedicineConsumtionId = 1,
                    TotalPrice = 100,
                    MedicineConsumtion = new MedicineConsumption
                    {
                        MedicineConsumptionId = 1,
                        MedicineInventoryId = 10,
                        Amount = 5,
                        ConsumptionDate = DateTime.Now,
                        Note = "Take after meals",
                        IsSpecialMedicine = false,
                        Status = true,
                        MedicineInventory = new MedicineInventory
                        {
                            Medicine = new Medicine
                            {
                                MedicineId = 101,
                                MedicineName = "Test Medicine",
                                DosageForm = "Tablet",
                                IsBhyt = true
                            },
                            BatchNumber = "BATCH001",
                            TransactionDate = DateTime.Now.AddMonths(-1),
                            ExpiryDate = DateTime.Now.AddMonths(6),
                            Quantity = 50
                        }
                    }
                },
                new PrescriptionMedicineConsumption
                {
                    PrescriptionId = prescriptionId,
                    MedicineConsumtionId = 2,
                    TotalPrice = 200,
                    MedicineConsumtion = new MedicineConsumption
                    {
                        MedicineConsumptionId = 2,
                        MedicineInventoryId = 20,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now,
                        Note = "Take before meals",
                        IsSpecialMedicine = true,
                        Status = false,
                        MedicineInventory = new MedicineInventory
                        {
                            Medicine = new Medicine
                            {
                                MedicineId = 102,
                                MedicineName = "Another Medicine",
                                DosageForm = "Syrup",
                                IsBhyt = false
                            },
                            BatchNumber = "BATCH002",
                            TransactionDate = DateTime.Now.AddMonths(-2),
                            ExpiryDate = DateTime.Now.AddMonths(3),
                            Quantity = 30
                        }
                    }
                }
            };

            // Create a mock DbSet with async support
            var mockPmcDbSet = new Mock<DbSet<PrescriptionMedicineConsumption>>();
            var queryablePmc = prescriptionMedicineConsumptions.AsQueryable();

            mockPmcDbSet.As<IAsyncEnumerable<PrescriptionMedicineConsumption>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<PrescriptionMedicineConsumption>(queryablePmc.GetEnumerator()));

            mockPmcDbSet.As<IQueryable<PrescriptionMedicineConsumption>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<PrescriptionMedicineConsumption>(queryablePmc.Provider));

            mockPmcDbSet.As<IQueryable<PrescriptionMedicineConsumption>>()
                .Setup(m => m.Expression)
                .Returns(queryablePmc.Expression);

            mockPmcDbSet.As<IQueryable<PrescriptionMedicineConsumption>>()
                .Setup(m => m.ElementType)
                .Returns(queryablePmc.ElementType);

            mockPmcDbSet.As<IQueryable<PrescriptionMedicineConsumption>>()
                .Setup(m => m.GetEnumerator())
                .Returns(queryablePmc.GetEnumerator());

            // Set up the mock DbContext
            var mockDbContext = new Mock<SEP_TestContext>();
            mockDbContext.Setup(c => c.PrescriptionMedicineConsumptions)
                .Returns(mockPmcDbSet.Object);

            // Replace the service's DbContext with our mock
            typeof(PrescriptionService).GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_service, mockDbContext.Object);

            // Act
            var result = await _service.GetPrescriptionDetailAsync(prescriptionId);

            // Assert
            Assert.Equal(prescriptionId, result.PrescriptionId);
            Assert.Equal(prescription.IssueDate, result.IssueDate);
            Assert.True(result.Status);
            Assert.Equal("Test prescription", result.Note);
            Assert.Equal("Test Doctor", result.FullName);
            Assert.Equal("Test Patient", result.PatientName);
            Assert.Equal("Male", result.Gender);
            Assert.Equal(new DateTime(1990, 1, 1), result.Dob);
            Assert.Equal("Test Address", result.Address);
            Assert.Equal("123456789", result.HealthInsurance);
            Assert.Equal("Test Diagnosis", result.DiagnoseConclusion);
            Assert.True(result.IsBhyt);

            // Check medicine consumptions
            Assert.Equal(2, result.MedicineConsumptions.Count);
            Assert.Equal(1, result.MedicineConsumptions[0].MedicineConsumptionId);
            Assert.Equal(101, result.MedicineConsumptions[0].MedicineId);
            Assert.Equal(5, result.MedicineConsumptions[0].Amount);
            Assert.Equal("Take after meals", result.MedicineConsumptions[0].Note);
            Assert.False(result.MedicineConsumptions[0].IsSpecialMedicine);
            Assert.True(result.MedicineConsumptions[0].Status);
            Assert.Equal("Test Medicine", result.MedicineConsumptions[0].MedicineName);
            Assert.Equal("Tablet", result.MedicineConsumptions[0].DosageForm);
            Assert.Equal("BATCH001", result.MedicineConsumptions[0].BatchNumber);
            Assert.Equal(100m, result.MedicineConsumptions[0].TotalPrice);

            // Check total price
            Assert.Equal(300m, result.TotalPrice); // 100 + 200
        }

        [Fact]
        public async Task GetPrescriptionDetailAsync_ThrowsExceptionWhenPrescriptionNotFound()
        {
            // Arrange
            int prescriptionId = 999;
            _repositoryMock.Setup(r => r.GetPrescriptionDetailAsync(prescriptionId))
                .ReturnsAsync((Prescription)null); // Prescription not found

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetPrescriptionDetailAsync(prescriptionId));
            Assert.Contains("Không tìm thấy đơn thuốc", ex.Message);
        }

        [Fact]
        public void GetTodayPrescriptionCount_ReturnsCorrectCount()
        {
            // Arrange
            int expectedCount = 5;
            _repositoryMock.Setup(r => r.CountTodayPrescriptions())
                .Returns(expectedCount);

            // Act
            var result = _service.GetTodayPrescriptionCount();

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetAllMedicineConsumptionsAsync_ReturnsCorrectlyMappedData()
        {
            // Arrange
            var testData = new List<PrescriptionMedicineConsumption>
    {
        new PrescriptionMedicineConsumption
        {
            PrescriptionId = 1,
            MedicineConsumtionId = 1,
            TotalPrice = 100,
            MedicineConsumtion = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                MedicineInventoryId = 10,
                Amount = 5,
                ConsumptionDate = DateTime.Now,
                Note = "Test note",
                Status = true,
                MedicineInventory = new MedicineInventory
                {
                    Medicine = new Medicine
                    {
                        MedicineName = "Test Medicine",
                        MedicineCode = "MED001",
                        ActiveIngredient = "Test Ingredient",
                        Dosage = "10mg",
                        DosageForm = "Tablet"
                    },
                    BatchNumber = "BATCH001",
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    TransactionDate = DateTime.Now.AddDays(-30)
                }
            }
        },
        new PrescriptionMedicineConsumption
        {
            PrescriptionId = 2,
            MedicineConsumtionId = 2,
            TotalPrice = 200,
            MedicineConsumtion = new MedicineConsumption
            {
                MedicineConsumptionId = 2,
                MedicineInventoryId = 20,
                Amount = 10,
                ConsumptionDate = DateTime.Now.AddDays(-1),
                Note = "Another note",
                Status = false,
                MedicineInventory = new MedicineInventory
                {
                    Medicine = new Medicine
                    {
                        MedicineName = "Another Medicine",
                        MedicineCode = "MED002",
                        ActiveIngredient = "Another Ingredient",
                        Dosage = "20mg",
                        DosageForm = "Syrup"
                    },
                    BatchNumber = "BATCH002",
                    ExpiryDate = DateTime.Now.AddMonths(3),
                    TransactionDate = DateTime.Now.AddDays(-60)
                }
            }
        }
    };

            _repositoryMock.Setup(r => r.GetAllMedicineConsumptionsAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllMedicineConsumptionsAsync();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].MedicineConsumptionId);
            Assert.Equal(10, result[0].MedicineInventoryId);
            Assert.Equal("Test Medicine", result[0].MedicineName);
            Assert.Equal("MED001", result[0].MedicineCode);
            Assert.Equal("Test Ingredient", result[0].ActiveIngredient);
            Assert.Equal("10mg", result[0].Dosage);
            Assert.Equal("Tablet", result[0].DosageForm);
            Assert.Equal("BATCH001", result[0].BatchNumber);
            Assert.Equal(5, result[0].Amount);
            Assert.Equal(testData[0].MedicineConsumtion.ConsumptionDate, result[0].ConsumptionDate);
            Assert.Equal(testData[0].MedicineConsumtion.MedicineInventory.ExpiryDate, result[0].ExpiryDate);
            Assert.Equal(testData[0].MedicineConsumtion.MedicineInventory.TransactionDate, result[0].TransactionDate);
            Assert.Equal("Test note", result[0].Note);
            Assert.True(result[0].Status);
            Assert.Equal(100, result[0].TotalPrice);
            Assert.Equal(1, result[0].PrescriptionId);

            Assert.Equal(2, result[1].MedicineConsumptionId);
            Assert.Equal("Another Medicine", result[1].MedicineName);
            Assert.Equal(200, result[1].TotalPrice);
            Assert.Equal(2, result[1].PrescriptionId);
        }

        #endregion

        private CreatePrescriptionDTO GetValidDTO()
        {
            return new CreatePrescriptionDTO
            {
                IssueDate = DateTime.Now,
                Note = "Test",
                IsBhyt = false,
                MedicineConsumptions = new List<MedicineConsumptionDTO>
            {
                new MedicineConsumptionDTO
                {
                    MedicineInventoryId = 1,
                    Amount = 5,
                    ConsumptionDate = DateTime.Now,
                    IsSpecialMedicine = false,
                    Note = "Take after meals"
                }
            }
            };
        }

        // ----------------------------------------
        // 🔧 Helper Methods
        // ----------------------------------------

        private void SetupInventory(int inventoryId, int medicineId, int quantity, DateTime expiry)
        {
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(inventoryId))
                .ReturnsAsync(new MedicineInventory
                {
                    MedicineInventoryId = inventoryId,
                    MedicineId = medicineId,
                    Quantity = quantity,
                    ExpiryDate = expiry
                });
        }

        private EditPrescriptionForDoctorDTO GetValidEditPrescriptionDTO()
        {
            return new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = DateTime.Now,
                Note = "Updated test prescription",
                IsBhyt = true,
                MedicineConsumptionIdsToRemove = new List<int> { 3 },
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
        {
            new MedicineConsumptionDTO
            {
                MedicineInventoryId = 2,
                Amount = 5,
                ConsumptionDate = DateTime.Now,
                IsSpecialMedicine = false,
                Note = "Take after meals"
            }
        }
            };
        }

        private Prescription GetTestPrescription()
        {
            return new Prescription
            {
                PrescriptionId = 1,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = DateTime.Now.AddDays(-1),
                Note = "Original test prescription",
                IsBhyt = false
            };
        }

        private EditPrescriptionForPharmacistDTO GetValidEditPrescriptionForPharmacistDTO(bool status)
        {
            return new EditPrescriptionForPharmacistDTO
            {
                PrescriptionId = 1,
                MedicineConsumptionStatuses = new List<MedicineConsumptionStatusDTO>
        {
            new MedicineConsumptionStatusDTO
            {
                MedicineConsumptionId = 1,
                Status = status
            }
        }
            };
        }

        // Helper classes for async testing
        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_inner.MoveNext());
            }

            public T Current => _inner.Current;
        }

        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(
                        name: nameof(IQueryProvider.Execute),
                        genericParameterCount: 1,
                        types: new[] { typeof(Expression) })
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(this, new[] { expression });

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult });
            }
        }

        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
    }
}