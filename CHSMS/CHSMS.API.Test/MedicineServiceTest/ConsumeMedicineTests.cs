using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumeMedicineTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public ConsumeMedicineTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            _service = new MedicineService(_mockRepo.Object, _loggerMock.Object);
        }

        [Fact]
        public void ConsumeMedicine_ValidInput_ReturnsSuccess()
        {
            // Arrange
            var inventory = new MedicineInventory { MedicineInventoryId = 1, Quantity = 5 };
            var consumeDto = new ConsumeMedicineDTO
            {
                MedicineInventoryId = 1,
                Quantity = 5
            };

            _mockRepo.Setup(x => x.GetMedicineInventoryById(1))
                .Returns(inventory);
            _mockRepo.Setup(x => x.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()))
                .Returns(true);
            _mockRepo.Setup(x => x.ConsumeMedicineByMedicineId(It.IsAny<ConsumeMedicineDTO>()))
                .Returns(1);

            // Act
            var result = _service.ConsumeMedicine(consumeDto);

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(0, inventory.Quantity); // Quantity should be reduced
        }

        [Fact]
        public void ConsumeMedicine_InvalidInventoryId_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetMedicineInventoryById(-1))
                .Returns((MedicineInventory)null);

            var consumeDto = new ConsumeMedicineDTO
            {
                MedicineInventoryId = -1,
                Quantity = 5
            };

            // Act
            var result = _service.ConsumeMedicine(consumeDto);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ConsumeMedicine_QuantityExceedsAvailable_ReturnsInsufficientQuantity()
        {
            // Arrange
            var inventory = new MedicineInventory { MedicineInventoryId = 1, Quantity = 5 };
            var consumeDto = new ConsumeMedicineDTO
            {
                MedicineInventoryId = 1,
                Quantity = 10
            };

            _mockRepo.Setup(x => x.GetMedicineInventoryById(1))
                .Returns(inventory);

            // Act
            var result = _service.ConsumeMedicine(consumeDto);

            // Assert
            Assert.Equal(-3, result);
            Assert.Equal(5, inventory.Quantity); // Quantity should remain unchanged
        }

        [Fact]
        public void ConsumeMedicine_NegativeQuantity_ReturnsInvalidQuantity()
        {
            // Arrange
            var inventory = new MedicineInventory { MedicineInventoryId = 1, Quantity = 5 };
            var consumeDto = new ConsumeMedicineDTO
            {
                MedicineInventoryId = 1,
                Quantity = -1
            };

            _mockRepo.Setup(x => x.GetMedicineInventoryById(1))
                .Returns(inventory);

            // Act
            var result = _service.ConsumeMedicine(consumeDto);

            // Assert
            Assert.Equal(-2, result);
            Assert.Equal(5, inventory.Quantity); // Quantity should remain unchanged
        }

        [Fact]
        public void ConsumeMedicine_RepositoryUpdateFails_ReturnsFailure()
        {
            // Arrange
            var inventory = new MedicineInventory { MedicineInventoryId = 1, Quantity = 5 };
            var consumeDto = new ConsumeMedicineDTO
            {
                MedicineInventoryId = 1,
                Quantity = 5
            };

            _mockRepo.Setup(x => x.GetMedicineInventoryById(1))
                .Returns(inventory);
            _mockRepo.Setup(x => x.UpdateMedicineInInventory(It.IsAny<List<MedicineInventory>>()))
                .Returns(false);
            _mockRepo.Setup(x => x.ConsumeMedicineByMedicineId(It.IsAny<ConsumeMedicineDTO>()))
                .Returns(1);

            // Act
            var result = _service.ConsumeMedicine(consumeDto);

            // Assert
            Assert.Equal(0, result);
            Assert.Equal(5, inventory.Quantity); // Quantity should remain unchanged
        }
    }
}
