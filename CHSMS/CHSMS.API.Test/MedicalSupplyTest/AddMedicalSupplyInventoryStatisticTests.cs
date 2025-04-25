using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class AddMedicalSupplyInventoryStatisticTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public AddMedicalSupplyInventoryStatisticTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_ReturnsTrue_WhenValidInput()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO() };
            var emptyList = new List<MedicalSupplyInventoryStatistic>();
            _mockRepository.Setup(repo => repo.GetAllMSISNotConfirm()).Returns(emptyList);
            _mockRepository.Setup(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>())).Returns(true);

            // Act
            var result = _service.AddMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_ReturnsFalse_WhenDTOListIsEmpty()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO>();

            // Act
            var result = _service.AddMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_ThrowsException_WhenRequiredFieldsAreNull()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { new MSIStatisticDTO { Msisid = 1 } }; // Missing required fields

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(dtos));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventoryStatistic_ThrowsException_WhenDuplicateExists()
        {
            // Arrange
            var dtos = new List<MSIStatisticDTO> { CreateMSIStatisticDTO() };
            var existing = new List<MedicalSupplyInventoryStatistic> { new MedicalSupplyInventoryStatistic { MsinventoryId = 1 } };
            _mockRepository.Setup(repo => repo.GetAllMSISNotConfirm()).Returns(existing);

            // Act & Assert
            Assert.Throws<Exception>(() => _service.AddMedicalSupplyInventoryStatistic(dtos));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
        }

        private MSIStatisticDTO CreateMSIStatisticDTO()
        {
            return new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = 50.0,
                ActualQuantity = 48.0,
                StatisticPerson = 101,
                StatisticDate = DateTime.Now,
                IsUpdate = false,
                Note = "Statistic note"
            };
        }
    }
}