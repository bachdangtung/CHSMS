using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class UpdateMedicineConsumptionTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public UpdateMedicineConsumptionTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void UpdateMedicineConsumption_UpdatesSuccessfully()
        {
            // Arrange
            var consumption = TestHelper.CreateMedicineConsumption(1, 1, 50);
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100);
            var dto = TestHelper.CreateConsumeMedicineDTO(1, 1, 75);

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1)).Returns(consumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1)).Returns(inventory);
            _medicineRepositoryMock.Setup(repo => repo.UpdateMedicineInventory(It.IsAny<MedicineInventory>())).Returns(true);
            _medicineRepositoryMock.Setup(repo => repo.UpdateMedicineConsumption(It.IsAny<MedicineConsumption>())).Returns(true);

            // Act
            var result = _service.UpdateMedicineConsumption(dto);

            // Assert
            Assert.True(result);
            Assert.Equal(75, inventory.Quantity);
            Assert.Equal(75, consumption.Amount);
        }

        [Fact]
        public void UpdateMedicineConsumption_ReturnsFalseWhenDtoIsNull()
        {
            // Act
            var result = _service.UpdateMedicineConsumption(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicineConsumption_ReturnsFalseWhenConsumptionNotFound()
        {
            // Arrange
            var dto = TestHelper.CreateConsumeMedicineDTO(1, 1, 75);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1)).Returns((MedicineConsumption)null);

            // Act
            var result = _service.UpdateMedicineConsumption(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicineConsumption_ReturnsFalseWhenInventoryNotFound()
        {
            // Arrange
            var consumption = TestHelper.CreateMedicineConsumption(1, 1, 50);
            var dto = TestHelper.CreateConsumeMedicineDTO(1, 1, 75);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1)).Returns(consumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1)).Returns((MedicineInventory)null);

            // Act
            var result = _service.UpdateMedicineConsumption(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicineConsumption_ReturnsFalseWhenInsufficientQuantity()
        {
            // Arrange
            var consumption = TestHelper.CreateMedicineConsumption(1, 1, 50);
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 20);
            var dto = TestHelper.CreateConsumeMedicineDTO(1, 1, 75);

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1)).Returns(consumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1)).Returns(inventory);

            // Act
            var result = _service.UpdateMedicineConsumption(dto);

            // Assert
            Assert.False(result);
        }
    }
}
