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

            // Setup precondition: MedicalSupplyInventory with MedicalSupplyInventoryId: 1, Quantity: 5 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(new MedicalSupplyInventory
                {
                    SupplyInventoryId = 1,
                    Quantity = 5,
                    ImportQuantity = 5,
                    MedicalSupplyId = 1
                });
        }

        [Fact]
        public void ConsumeMedicalSupply_ValidIdAndQuantity_Returns1()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };

            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);
            _mockRepository.Setup(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO))
                .Returns(1);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(1, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(consumpMSDTO), Times.Once());
        }

        [Fact]
        public void ConsumeMedicalSupply_InvalidId_ReturnsMinus1()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = -1,
                Quantity = 5,
                Status = true,
                Note = "Test consumption"
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(-1))
                .Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-1, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }

        [Fact]
        public void ConsumeMedicalSupply_ExcessiveQuantity_ReturnsMinus3()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = 10,
                Status = true,
                Note = "Test consumption"
            };

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-3, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }

        [Fact]
        public void ConsumeMedicalSupply_NegativeQuantity_ReturnsMinus2()
        {
            // Arrange
            var consumpMSDTO = new ConsumpMSDTO
            {
                MedicalSupplyInventoryId = 1,
                Quantity = -1,
                Status = true,
                Note = "Test consumption"
            };

            // Act
            var result = _service.ConsumeMedicalSupply(consumpMSDTO);

            // Assert
            Assert.Equal(-2, result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.ConsumeMedicalSupplyByMSID(It.IsAny<ConsumpMSDTO>()), Times.Never());
        }
    }
}