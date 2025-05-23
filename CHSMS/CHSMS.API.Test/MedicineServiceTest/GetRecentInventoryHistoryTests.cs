using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetRecentInventoryHistoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetRecentInventoryHistoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetRecentInventoryHistory_ReturnsInventories()
        {
            // Arrange
            var inventories = new List<MedicineInventory>
            {
                TestHelper.CreateMedicineInventory(1, 1, 100, "BATCH001"),
                TestHelper.CreateMedicineInventory(2, 1, 200, "BATCH002")
            };
            _medicineRepositoryMock.Setup(repo => repo.GetRecentInventoriesByUser(1)).Returns(inventories);

            // Act
            var result = _service.GetRecentInventoryHistory(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(100, result[0].Quantity);
            Assert.Equal("BATCH001", result[0].BatchNumber);
        }

        [Fact]
        public void GetRecentInventoryHistory_ReturnsEmptyListWhenNoInventories()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetRecentInventoriesByUser(1)).Returns(new List<MedicineInventory>());

            // Act
            var result = _service.GetRecentInventoryHistory(-1);

            // Assert
            Assert.Empty(result);
        }
    }
}
