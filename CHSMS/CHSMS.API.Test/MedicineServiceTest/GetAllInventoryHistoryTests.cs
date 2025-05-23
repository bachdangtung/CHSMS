using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllInventoryHistoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetAllInventoryHistoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllInventoryHistory_ReturnsHistoryWithEditPermissions()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100, "BATCH001");
            inventory.TransactionDate = DateTime.Now.AddHours(-12);
            var inventories = new List<MedicineInventory> { inventory };
            _medicineRepositoryMock.Setup(repo => repo.GetAllInventoriesByUser(1)).Returns(inventories);

            // Act
            var result = _service.GetAllInventoryHistory(1);

            // Assert
            Assert.Single(result);
            Assert.True(result[0].CanEdit);
            Assert.True(result[0].CanEditNote);
            Assert.True(result[0].CanEditImportQuantity);
            Assert.True(result[0].CanEditManufacturingDate);
            Assert.Equal(100, result[0].Quantity);
        }

        [Fact]
        public void GetAllInventoryHistory_DeniesEditForOldRecords()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100, "BATCH001");
            inventory.TransactionDate = DateTime.Now.AddHours(-25);
            var inventories = new List<MedicineInventory> { inventory };
            _medicineRepositoryMock.Setup(repo => repo.GetAllInventoriesByUser(1)).Returns(inventories);

            // Act
            var result = _service.GetAllInventoryHistory(1);

            // Assert
            Assert.Single(result);
            Assert.False(result[0].CanEdit);
            Assert.False(result[0].CanEditNote);
            Assert.False(result[0].CanEditImportQuantity);
            Assert.False(result[0].CanEditManufacturingDate);
        }

        [Fact]
        public void GetAllInventoryHistory_ReturnsEmptyListWhenNoHistory()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetAllInventoriesByUser(-1)).Returns(new List<MedicineInventory>());

            // Act
            var result = _service.GetAllInventoryHistory(-1);

            // Assert
            Assert.Empty(result);
        }
    }
}
