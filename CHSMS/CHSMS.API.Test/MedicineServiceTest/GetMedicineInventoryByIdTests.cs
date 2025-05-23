using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineInventoryByIdTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetMedicineInventoryByIdTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineInventoryById_ReturnsInventory()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1)).Returns(inventory);

            // Act
            var result = _service.GetMedicineInventoryById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicineInventoryId);
            Assert.Equal(100, result.Quantity);
        }

        [Fact]
        public void GetMedicineInventoryById_ReturnsNullWhenNotFound()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1)).Returns((MedicineInventory)null);

            // Act
            var result = _service.GetMedicineInventoryById(1);

            // Assert
            Assert.Null(result);
        }
    }
}
