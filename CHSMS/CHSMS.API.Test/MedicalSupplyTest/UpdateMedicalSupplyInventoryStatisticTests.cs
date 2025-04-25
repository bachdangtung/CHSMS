using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class UpdateMedicalSupplyInventoryStatisticTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public UpdateMedicalSupplyInventoryStatisticTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ReturnsTrue_WhenUpdateSucceedsWithInventory()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO(true) };
            var inventory = new MedicalSupplyInventory { SupplyInventoryId = 1, Quantity = 50.0 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>())).Returns(true);
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>())).Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.True(result);
            Assert.Equal(48.0, inventory.Quantity);
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ReturnsTrue_WhenUpdateSucceedsWithoutInventory()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO(false) };
            var inventory = new MedicalSupplyInventory { SupplyInventoryId = 1, Quantity = 50.0 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>())).Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ReturnsFalse_WhenDTOListIsEmpty()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO>();

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ThrowsException_WhenInventoryNotFound()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO(true) };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns((MedicalSupplyInventory)null);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventoryStatistic(dtos));
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ThrowsException_WhenQuantityMismatch()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO(true, 60.0) };
            var inventory = new MedicalSupplyInventory { SupplyInventoryId = 1, Quantity = 50.0 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventoryStatistic(dtos));
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        private MSIStatisticDTO CreateMSIStatisticDTO(bool isUpdate, double quantity = 50.0)
        {
            return new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = quantity,
                ActualQuantity = 48.0,
                StatisticPerson = 101,
                StatisticDate = DateTime.Now,
                IsUpdate = isUpdate,
                Note = "Statistic note"
            };
        }
    }
}