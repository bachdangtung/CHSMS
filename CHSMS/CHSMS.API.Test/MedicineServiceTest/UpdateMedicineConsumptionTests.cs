using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class UpdateMedicineConsumptionTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly IMedicineService _medicineService;

        public UpdateMedicineConsumptionTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void UpdateMedicineConsumption_ValidInput_ReturnsTrue()
        {
            // Arrange
            var consumeMedicineDTO = new ConsumeMedicineDTO
            {
                ConsumeMedicineId = 1,
                MedicineInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Updated consumption"
            };

            var medicineConsumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                MedicineInventoryId = 1,
                Amount = 2, // Previous consumption
                Status = true,
                Note = "Initial consumption"
            };

            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 10
            };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1))
                .Returns(medicineConsumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1))
                .Returns(medicineInventory);
            _medicineRepositoryMock.Setup(repo => repo.UpdateMedicineInventory(It.IsAny<MedicineInventory>()))
                .Returns(true);
            _medicineRepositoryMock.Setup(repo => repo.UpdateMedicineConsumption(It.IsAny<MedicineConsumption>()))
                .Returns(true);

            // Act
            var result = _medicineService.UpdateMedicineConsumption(consumeMedicineDTO);

            // Assert
            Assert.True(result);
            Assert.Equal(7, medicineInventory.Quantity); // 10 - (5 - 2) = 7
            Assert.Equal(5, medicineConsumption.Amount);
            Assert.Equal(consumeMedicineDTO.Status, medicineConsumption.Status);
            Assert.Equal(consumeMedicineDTO.Note, medicineConsumption.Note);
        }

        [Fact]
        public void UpdateMedicineConsumption_InvalidConsumptionId_ReturnsFalse()
        {
            // Arrange
            var consumeMedicineDTO = new ConsumeMedicineDTO
            {
                ConsumeMedicineId = -1,
                MedicineInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Test"
            };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(-1))
                .Returns((MedicineConsumption)null);

            // Act
            var result = _medicineService.UpdateMedicineConsumption(consumeMedicineDTO);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicineConsumption_InvalidInventoryId_ReturnsFalse()
        {
            // Arrange
            var consumeMedicineDTO = new ConsumeMedicineDTO
            {
                ConsumeMedicineId = 1,
                MedicineInventoryId = -1,
                Quantity = 5,
                Status = true,
                Note = "Test"
            };

            var medicineConsumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                MedicineInventoryId = -1,
                Amount = 2
            };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1))
                .Returns(medicineConsumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(-1))
                .Returns((MedicineInventory)null);

            // Act
            var result = _medicineService.UpdateMedicineConsumption(consumeMedicineDTO);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicineConsumption_InsufficientQuantity_ReturnsFalse()
        {
            // Arrange
            var consumeMedicineDTO = new ConsumeMedicineDTO
            {
                ConsumeMedicineId = 1,
                MedicineInventoryId = 1,
                Quantity = 15,
                Status = true,
                Note = "Test"
            };

            var medicineConsumption = new MedicineConsumption
            {
                MedicineConsumptionId = 1,
                MedicineInventoryId = 1,
                Amount = 2
            };

            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = 1,
                Quantity = 10
            };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineConsumptionById(1))
                .Returns(medicineConsumption);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineInventoryById(1))
                .Returns(medicineInventory);

            // Act
            var result = _medicineService.UpdateMedicineConsumption(consumeMedicineDTO);

            // Assert
            Assert.False(result);
        }
    }
}
