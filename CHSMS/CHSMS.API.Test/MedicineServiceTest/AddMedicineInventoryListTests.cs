using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class AddMedicineInventoryListTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public AddMedicineInventoryListTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void AddMedicineInventoryList_ValidDTOList_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var transactionDate = new DateTime(2025, 4, 4);
            var dtoList = new List<MedicineInventoryAddDTO>
        {
            new MedicineInventoryAddDTO
            {
                MedicineId = 1,
                BatchNumber = "XYZ123",
                TransactionDate = transactionDate,
                ImportQuantity = 100,
                ManufacturingDate = new DateTime(2025, 1, 1),
                CertificateNumber = "CERT123",
                SupplierId = 1,
                Note = "Test note",
                TransactionType = true
            }
        };

            var medicine = new Medicine { MedicineId = 1, ShelfLife = 12 };
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns(medicine);
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "XYZ123", transactionDate)).Returns(false);
            _medicineRepositoryMock.Setup(repo => repo.CalculateExpiryDate(It.IsAny<DateTime>(), 12)).Returns(new DateTime(2026, 1, 1));
            _medicineRepositoryMock.Setup(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>())).Returns(true);

            // Act
            var result = _medicineService.AddMedicineInventoryList(dtoList, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.AddedCount);
            Assert.Empty(result.Warnings);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryList(It.Is<List<MedicineInventory>>(list =>
                list.Count == 1 &&
                list[0].MedicineId == 1 &&
                list[0].BatchNumber == "XYZ123" &&
                list[0].Quantity == 100 &&
                list[0].ReceiverId == userId)));
            _medicineRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never());
        }

        [Fact]
        public void AddMedicineInventoryList_NullDTOList_ReturnsEmptyResultWithWarning()
        {
            // Arrange
            var userId = 1;
            List<MedicineInventoryAddDTO> dtoList = null;

            // Act
            var result = _medicineService.AddMedicineInventoryList(dtoList, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains("Danh sách DTO trống.", result.Warnings);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _loggerMock.Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AddMedicineInventoryList_DuplicateBatchNumber_ReturnsWarning()
        {
            // Arrange
            var userId = 1;
            var transactionDate = new DateTime(2025, 4, 4);
            var dtoList = new List<MedicineInventoryAddDTO>
        {
            new MedicineInventoryAddDTO
            {
                MedicineId = 1,
                BatchNumber = "ABCD1234",
                TransactionDate = transactionDate,
                ImportQuantity = 100,
                ManufacturingDate = new DateTime(2025, 1, 1),
                CertificateNumber = "CERT123",
                SupplierId = 1,
                Note = "Test note",
                TransactionType = true
            }
        };

            var medicine = new Medicine { MedicineId = 1, ShelfLife = 12 };
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns(medicine);
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "ABCD1234", transactionDate)).Returns(true);

            // Act
            var result = _medicineService.AddMedicineInventoryList(dtoList, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains($"Thuốc ID 1 với số lô ABCD1234 đã nhập trong ngày 04/04/2025", result.Warnings);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _loggerMock.Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AddMedicineInventoryList_InvalidMedicineId_ReturnsWarning()
        {
            // Arrange
            var userId = 1;
            var transactionDate = new DateTime(2025, 4, 4);
            var dtoList = new List<MedicineInventoryAddDTO>
        {
            new MedicineInventoryAddDTO
            {
                MedicineId = -1,
                BatchNumber = "XYZ123",
                TransactionDate = transactionDate,
                ImportQuantity = 100,
                ManufacturingDate = new DateTime(2025, 1, 1),
                CertificateNumber = "CERT123",
                SupplierId = 1,
                Note = "Test note",
                TransactionType = true
            }
        };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(-1)).Returns((Medicine)null);
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(-1, "XYZ123", transactionDate)).Returns(false);

            // Act
            var result = _medicineService.AddMedicineInventoryList(dtoList, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains("Thuốc ID -1 không tồn tại.", result.Warnings);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _loggerMock.Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AddMedicineInventoryList_InvalidSupplierId_ReturnsWarning()
        {
            // Arrange
            var userId = 1;
            var transactionDate = new DateTime(2025, 4, 4);
            var dtoList = new List<MedicineInventoryAddDTO>
        {
            new MedicineInventoryAddDTO
            {
                MedicineId = 1,
                BatchNumber = "XYZ123",
                TransactionDate = transactionDate,
                ImportQuantity = 100,
                ManufacturingDate = new DateTime(2025, 1, 1),
                CertificateNumber = "CERT123",
                SupplierId = -1,
                Note = "Test note",
                TransactionType = true
            }
        };

            var medicine = new Medicine { MedicineId = 1, ShelfLife = 12 };
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns(medicine);
            _medicineRepositoryMock.Setup(repo => repo.CheckDuplicateBatch(1, "XYZ123", transactionDate)).Returns(false);

            // Act
            var result = _medicineService.AddMedicineInventoryList(dtoList, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(0, result.AddedCount);
            Assert.Contains("Nhà cung cấp không hợp lệ cho thuốc ID 1.", result.Warnings);
            _medicineRepositoryMock.Verify(repo => repo.AddMedicineInventoryList(It.IsAny<List<MedicineInventory>>()), Times.Never());
            _loggerMock.Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once());
        }
    }
}
