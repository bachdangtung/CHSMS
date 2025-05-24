using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class ConsumeMedicalSupplyTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public ConsumeMedicalSupplyTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void ConsumeMedicalSupply_ValidDTO_ReturnsOne()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                Quantity = 10,
                MedicalSupplyId = 1
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);
            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);
            _mockRepository.Setup(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO))
                .Returns(1);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(1, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.Is<List<MedicalSupplyInventory>>(list => list[0].Quantity == 5)), Times.Once());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO), Times.Once());
        }

        [Fact]
        public void ConsumeMedicalSupply_MedicalSupplyInventoryIdNotExist_ReturnsNegativeOne()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 999,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(999))
                .Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-1, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }

        [Fact]
        public void ConsumeMedicalSupply_QuantityExceedsInventory_ReturnsNegativeThree()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 15,
                Status = true,
                Note = "Test consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                Quantity = 10,
                MedicalSupplyId = 1
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-3, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }

        [Fact]
        public void ConsumeMedicalSupply_NegativeQuantity_ReturnsNegativeTwo()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = -5,
                Status = true,
                Note = "Test consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                Quantity = 10,
                MedicalSupplyId = 1
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-2, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }

        [Fact]
        public void ConsumeMedicalSupply_RepositoryUpdateFails_ReturnsZero()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                Quantity = 10,
                MedicalSupplyId = 1
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);
            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(false); // Simulate repository failure
            _mockRepository.Setup(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO))
                .Returns(1);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(0, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO), Times.Once());
        }

        [Fact]
        public void ConsumeMedicalSupply_RepositoryConsumeFails_ReturnsZero()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                Quantity = 10,
                MedicalSupplyId = 1
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(inventory);
            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);
            _mockRepository.Setup(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO))
                .Returns(0); // Simulate repository failure

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(0, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO), Times.Once());
        }
    }
}