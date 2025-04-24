using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class MedicineDetailTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public MedicineDetailTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void MedicineDetail_ReturnsInventoryDetails()
        {
            // Arrange
            var inventories = new List<MedicineInventory>
            {
                TestHelper.CreateMedicineInventory(1, 1, 100, "BATCH001"),
                TestHelper.CreateMedicineInventory(2, 1, 200, "BATCH002")
            };
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventory(1, false)).Returns(inventories);

            // Act
            var result = _service.MedicineDetail(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(100, result[0].Quantity);
            Assert.Equal("BATCH001", result[0].BatchNumber);
        }

        [Fact]
        public void MedicineDetail_ReturnsEmptyListWhenNoInventories()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventory(1, false)).Returns(new List<MedicineInventory>());

            // Act
            var result = _service.MedicineDetail(1);

            // Assert
            Assert.Empty(result);
        }
    }
}
