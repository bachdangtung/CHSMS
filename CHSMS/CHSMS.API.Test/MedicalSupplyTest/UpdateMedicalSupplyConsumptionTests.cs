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
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ReturnsTrue_WhenUpdateSucceeds()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 15.0,
                Status = true,
                Note = "Updated consumption"
            };
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 10.0,
                Status = true,
                Note = "Initial consumption"
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 50.0
            };
            _mockRepository.Setup(repo => repo.GetSupplyConsumptionByID(1)).Returns(consumption);
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>())).Returns(true);
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>())).Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.True(result);
            Assert.Equal(45.0, inventory.Quantity); // 50 - (15 - 10)
            Assert.Equal(15.0, consumption.Amount);
            Assert.Equal(dto.Status, consumption.Status);
            Assert.Equal(dto.Note, consumption.Note);
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(1), Times.Once());
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(1), Times.Once());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.Is<List<MedicalSupplyInventory>>(list => list[0].Quantity == 45.0)), Times.Once());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyConsumption(It.Is<MedicalSupplyConsumption>(c => c.Amount == 15.0)), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ReturnsFalse_WhenConsumptionNotFound()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 15.0
            };
            _mockRepository.Setup(repo => repo.GetSupplyConsumptionByID(1)).Returns((MedicalSupplyConsumption)null);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(1), Times.Once());
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(It.IsAny<int>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ReturnsFalse_WhenInventoryNotFound()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 15.0
            };
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 10.0
            };
            _mockRepository.Setup(repo => repo.GetSupplyConsumptionByID(1)).Returns(consumption);
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(1), Times.Once());
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(1), Times.Once());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ReturnsFalse_WhenQuantityResultsInNegativeInventory()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = 60.0
            };
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 10.0
            };
            var inventory = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 40.0
            };
            _mockRepository.Setup(repo => repo.GetSupplyConsumptionByID(1)).Returns(consumption);
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(1), Times.Once());
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(1), Times.Once());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ReturnsFalse_WhenDTOIsNull()
        {
            // Arrange
            ConsumpMSDTO dto = null;

            // Act
            var result = _service.UpdateMedicalSupplyConsumption(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(It.IsAny<int>()), Times.Never());
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(It.IsAny<int>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyConsumption(It.IsAny<MedicalSupplyConsumption>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ThrowsException_WhenConsumpMSIDIsNull()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = null,
                MedicalSupplyInventoryId = 1,
                Quantity = 15.0
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.UpdateMedicalSupplyConsumption(dto));
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ThrowsException_WhenMedicalSupplyInventoryIdIsNull()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = null,
                Quantity = 15.0
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.UpdateMedicalSupplyConsumption(dto));
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyConsumption_ThrowsException_WhenQuantityIsNull()
        {
            // Arrange
            var dto = new ConsumpMSDTO
            {
                ConsumpMSID = 1,
                MedicalSupplyInventoryId = 1,
                Quantity = null
            };
            var consumption = new MedicalSupplyConsumption
            {
                MsconsumptionId = 1,
                MedicalSupplyInventoryId = 1,
                Amount = 10.0
            };
            _mockRepository.Setup(repo => repo.GetSupplyConsumptionByID(1)).Returns(consumption);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.UpdateMedicalSupplyConsumption(dto));
            _mockRepository.Verify(repo => repo.GetSupplyConsumptionByID(1), Times.Once());
        }
    }
}