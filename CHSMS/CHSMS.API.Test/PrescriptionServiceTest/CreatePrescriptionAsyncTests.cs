using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class CreatePrescriptionAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<DatabaseFacade> _databaseFacadeMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly IPrescriptionService _service;

        public CreatePrescriptionAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _databaseFacadeMock = new Mock<DatabaseFacade>(_contextMock.Object);
            _transactionMock = new Mock<IDbContextTransaction>();

            // Thiết lập thuộc tính Database trả về DatabaseFacade mock
            _contextMock.Setup(c => c.Database).Returns(_databaseFacadeMock.Object);

            // Thiết lập BeginTransactionAsync trên DatabaseFacade
            _databaseFacadeMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(_transactionMock.Object);

            // Khởi tạo service với thứ tự tham số đúng
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_EmptyMedicineConsumptions_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>(),
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Đơn thuốc phải chứa ít nhất một loại thuốc!", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 10, ConsumptionDate = DateTime.Now }
                },
                IssueDate = DateTime.Now.AddDays(1),
                Note = "Test note",
                IsBhyt = false
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Ngày phát hành không được là ngày trong tương lai!", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_MoreThanTenMedicines_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = Enumerable.Range(1, 11).Select(i => new MedicineConsumptionDTO
                {
                    MedicineInventoryId = i,
                    Amount = 10,
                    ConsumptionDate = DateTime.Now
                }).ToList(),
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((int id) => new MedicineInventory
                           {
                               MedicineInventoryId = id,
                               MedicineId = id,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30),
                               Medicine = new Medicine { MedicineId = id }
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Một đơn thuốc không được chứa quá 10 loại thuốc!", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_DuplicateMedicineInventoryIds_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 10, ConsumptionDate = DateTime.Now },
                    new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 5, ConsumptionDate = DateTime.Now }
                },
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((int id) => new MedicineInventory
                           {
                               MedicineInventoryId = id,
                               MedicineId = id,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30),
                               Medicine = new Medicine { MedicineId = id }
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Có thuốc bị trùng trong đơn thuốc. Vui lòng kiểm tra lại!", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_ValidInput_CreatesPrescriptionSuccessfully()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO
                    {
                        MedicineInventoryId = 1,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now,
                        IsSpecialMedicine = false,
                        Note = "Test"
                    }
                },
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30),
                               Medicine = new Medicine { MedicineId = 1 }
                           });

            _repositoryMock.Setup(r => r.CreatePrescriptionAsync(It.IsAny<Prescription>()))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });

            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
 
                           .ReturnsAsync(new MedicineConsumption { MedicineConsumptionId = 1 });

            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .ReturnsAsync(new PrescriptionMedicineConsumption { PrescriptionId = 1, MedicineConsumtionId = 1 });

            // Act
            var result = await _service.CreatePrescriptionAsync(1, 1, dto);

            // Assert
            Assert.Equal(1, result);
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Never());
        }

        [Fact]
        public async Task CreatePrescriptionAsync_InsufficientInventory_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO
                    {
                        MedicineInventoryId = 1,
                        Amount = 200,
                        ConsumptionDate = DateTime.Now
                    }
                },
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30),
                               Medicine = new Medicine { MedicineId = 1 }
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Số lượng yêu cầu vượt quá tồn kho", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_ExpiredMedicine_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO
                    {
                        MedicineInventoryId = 1,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now.AddDays(1)
                    }
                },
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(-1),
                               Medicine = new Medicine { MedicineId = 1 }
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Ngày sử dụng vượt quá hạn sử dụng", exception.Message);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_NonExistentInventory_ThrowsException()
        {
            // Arrange
            var dto = new CreatePrescriptionDTO
            {
                MedicineConsumptions = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO
                    {
                        MedicineInventoryId = 2,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now
                    }
                },
                IssueDate = DateTime.Now,
                Note = "Test note",
                IsBhyt = false
            };

            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync((MedicineInventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionAsync(1, 1, dto));
            Assert.Contains("Không tìm thấy kho thuốc với ID: 2", exception.Message);
        }
    }

}