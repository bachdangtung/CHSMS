using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class UpdateMedicineInventoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public UpdateMedicineInventoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void UpdateMedicineInventory_ValidDTOAndUserId_Within24Hours_ReturnsTrue()
        {
            // Arrange
            var medicineInventoryId = 1;
            var userId = 1;
            var transactionDate = DateTime.Now.AddHours(-23); // Within 24 hours
            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                ReceiverId = userId,
                TransactionDate = transactionDate,
                Quantity = 100,
                ImportQuantity = 100,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                Note = "Initial note"
            };

            var dto = new MedicineInventoryUpdateDTO
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 90,
                ImportQuantity = 90,
                ManufacturingDate = DateTime.Now.AddMonths(-5),
                Note = "Updated note"
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetInventoryById(medicineInventoryId))
                .Returns(medicineInventory);
            _medicineRepositoryMock
                .Setup(repo => repo.SaveChanges())
                .Returns(true);

            // Act
            var result = _medicineService.UpdateMedicineInventory(dto, userId);

            // Assert
            Assert.True(result);
            _medicineRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once());
            Assert.Equal(90, medicineInventory.Quantity);
            Assert.Equal(90, medicineInventory.ImportQuantity);
            Assert.Equal("Updated note", medicineInventory.Note);
        }

        [Fact]
        public void UpdateMedicineInventory_InvalidMedicineInventoryId_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var dto = new MedicineInventoryUpdateDTO
            {
                MedicineInventoryId = -1,
                Quantity = 90,
                ImportQuantity = 90,
                ManufacturingDate = DateTime.Now.AddMonths(-5),
                Note = "Updated note"
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetInventoryById(-1))
                .Returns((MedicineInventory)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.UpdateMedicineInventory(dto, userId));
            Assert.Equal("Không tìm thấy bản ghi.", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never());
        }

        [Fact]
        public void UpdateMedicineInventory_UserIdMismatch_ThrowsException()
        {
            // Arrange
            var medicineInventoryId = 1;
            var userId = -1; // Invalid userId
            var transactionDate = DateTime.Now.AddHours(-23); // Within 24 hours
            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                ReceiverId = 1, // Different from userId
                TransactionDate = transactionDate
            };

            var dto = new MedicineInventoryUpdateDTO
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 90,
                ImportQuantity = 90,
                ManufacturingDate = DateTime.Now.AddMonths(-5),
                Note = "Updated note"
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetInventoryById(medicineInventoryId))
                .Returns(medicineInventory);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.UpdateMedicineInventory(dto, userId));
            Assert.Equal("Bạn không có quyền sửa bản ghi này.", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never());
        }

        [Fact]
        public void UpdateMedicineInventory_Over24Hours_ThrowsException()
        {
            // Arrange
            var medicineInventoryId = 1;
            var userId = 1;
            var transactionDate = DateTime.Now.AddHours(-25); // Over 24 hours
            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryId,
                ReceiverId = userId,
                TransactionDate = transactionDate
            };

            var dto = new MedicineInventoryUpdateDTO
            {
                MedicineInventoryId = medicineInventoryId,
                Quantity = 90,
                ImportQuantity = 90,
                ManufacturingDate = DateTime.Now.AddMonths(-5),
                Note = "Updated note"
            };

            _medicineRepositoryMock
                .Setup(repo => repo.GetInventoryById(medicineInventoryId))
                .Returns(medicineInventory);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _medicineService.UpdateMedicineInventory(dto, userId));
            Assert.Equal("Bản ghi đã quá 24 giờ, không thể chỉnh sửa.", exception.Message);
            _medicineRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never());
        }
    }
}
