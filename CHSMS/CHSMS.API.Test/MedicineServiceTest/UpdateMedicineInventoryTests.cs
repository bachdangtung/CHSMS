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
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public UpdateMedicineInventoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void UpdateMedicineInventory_UpdatesSuccessfully()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100);
            inventory.TransactionDate = DateTime.Now.AddHours(-12);
            inventory.ReceiverId = 1;
            var dto = TestHelper.CreateMedicineInventoryUpdateDTO(1, 1, 75);

            _medicineRepositoryMock.Setup(repo => repo.GetInventoryById(1)).Returns(inventory);
            _medicineRepositoryMock.Setup(repo => repo.SaveChanges()).Returns(true);

            // Act
            var result = _service.UpdateMedicineInventory(dto, 1);

            // Assert
            Assert.True(result);
            Assert.Equal(75, inventory.Quantity);
            Assert.Equal("Updated Note", inventory.Note);
        }

        [Fact]
        public void UpdateMedicineInventory_ThrowsWhenInventoryNotFound()
        {
            // Arrange
            var dto = TestHelper.CreateMedicineInventoryUpdateDTO(1, 1, 75);
            _medicineRepositoryMock.Setup(repo => repo.GetInventoryById(1)).Returns((MedicineInventory)null);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.UpdateMedicineInventory(dto, 1));
        }

        [Fact]
        public void UpdateMedicineInventory_ThrowsWhenUnauthorizedUser()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100);
            inventory.ReceiverId = 2;
            var dto = TestHelper.CreateMedicineInventoryUpdateDTO(1, 1, 75);
            _medicineRepositoryMock.Setup(repo => repo.GetInventoryById(1)).Returns(inventory);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.UpdateMedicineInventory(dto, 1));
        }

        [Fact]
        public void UpdateMedicineInventory_ThrowsWhenOver24Hours()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100);
            inventory.TransactionDate = DateTime.Now.AddHours(-25);
            inventory.ReceiverId = 1;
            var dto = TestHelper.CreateMedicineInventoryUpdateDTO(1, 1, 75);
            _medicineRepositoryMock.Setup(repo => repo.GetInventoryById(1)).Returns(inventory);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.UpdateMedicineInventory(dto, 1));
        }
    }
}
