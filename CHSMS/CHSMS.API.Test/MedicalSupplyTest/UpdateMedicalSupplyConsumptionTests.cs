using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class UpdateMedicalSupplyConsumptionTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public UpdateMedicalSupplyConsumptionTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup preconditions
            _mockRepository.Setup(r => r.GetSupplyConsumptionByID(1))
                .Returns(new MedicalSupplyConsumption
                {
                    MsconsumptionId = 1,
                    MedicalSupplyInventoryId = 1,
                    Amount = 50,
                    ConsumptionDate = DateTime.Now,
                    Status = true
                });
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(new MedicalSupplyInventory
                {
                    SupplyInventoryId = 1,
                    MedicalSupplyId = 1,
                    Quantity = 100,
                    ImportQuantity = 100
                });
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ValidDTO_ReturnsTrue()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 60, // Increasing from 50 to 60
                Status = true,
                Note = "Updated"
            };

            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);
            _mockRepository.Setup(r => r.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()))
                .Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.Is<List<MedicalSupplyInventory>>(list =>
                list[0].SupplyInventoryId == 1 && list[0].Quantity == 90)), Times.Once());
            _mockRepository.Verify(r => r.UpdateMedicalSupplyConsumption(It.Is<MedicalSupplyConsumption>(msc =>
                msc.MsconsumptionId == 1 && msc.Amount == 60 && msc.Status == true && msc.Note == "Updated")), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_InvalidConsumpMSID_ReturnsFalse()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = -1,
                MedicalSupplyInventoryId = 1,
                Quantity = 60,
                Status = true,
                Note = "Test"
            };

            _mockRepository.Setup(r => r.GetSupplyConsumptionByID(-1))
                .Returns((MedicalSupplyConsumption)null);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_InvalidMedicalSupplyInventoryId_ReturnsFalse()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = -1,
                Quantity = 60,
                Status = true,
                Note = "Test"
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(-1))
                .Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_NegativeQuantity_ReturnsFalse()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = -1,
                Status = true,
                Note = "Test"
            };

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_QuantityExceedsInventory_ReturnsFalse()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 200, // Exceeds available inventory (100)
                Status = true,
                Note = "Test"
            };

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(r => r.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()), Times.Never());
        }
    }
}