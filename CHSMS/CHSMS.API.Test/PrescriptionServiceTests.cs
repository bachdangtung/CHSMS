using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

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

        // ----------------------------------------
        // 🔧 Helper Methods
        // ----------------------------------------

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
    }
}