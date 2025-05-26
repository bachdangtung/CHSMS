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

            // Setup precondition: MedicalSupplyInventory with SupplyInventoryId: 1, Quantity: 20 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(new MedicalSupplyInventory { SupplyInventoryId = 1, Quantity = 20, MedicalSupplyId = 1 });
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ValidDTOList_ReturnsTrue()
        {
            // Arrange
            var dto = new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = false,
                Note = "Test statistic"
            };
            var dtos = new List<MSIStatisticDTO> { dto };

            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()))
                .Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_ValidDTOListWithUpdate_ReturnsTrue()
        {
            // Arrange
            var dto = new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = true,
                Note = "Test statistic with update"
            };
            var dtos = new List<MSIStatisticDTO> { dto };

            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()))
                .Returns(true);
            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Once());
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_NullDTOList_ReturnsFalse()
        {
            // Arrange
            List<MSIStatisticDTO> dtos = null;

            // Act
            var result = _service.UpdateMedicalSupplyInventoryStatistic(dtos);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventoryStatistic(It.IsAny<List<MedicalSupplyInventoryStatistic>>()), Times.Never());
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }



        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_InvalidMsinventoryId_ThrowsException()
        {
            // Arrange
            var dto = new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = -1,
                Quantity = 20,
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = false,
                Note = "Test statistic"
            };
            var dtos = new List<MSIStatisticDTO> { dto };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(-1))
                .Returns((MedicalSupplyInventory)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventoryStatistic(dtos));
            Assert.Equal("Vật tư không hợp lệ", exception.Message);
        }

        [Fact]
        public void UpdateMedicalSupplyInventoryStatistic_MismatchedQuantity_ThrowsException()
        {
            // Arrange
            var dto = new MSIStatisticDTO
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = 30, // Mismatches MedicalSupplyInventory.Quantity (20)
                ActualQuantity = 18,
                StatisticPerson = 1,
                StatisticDate = DateTime.Now,
                IsUpdate = true,
                Note = "Test statistic with update"
            };
            var dtos = new List<MSIStatisticDTO> { dto };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventoryStatistic(dtos));
            Assert.Equal("Số lượng hệ thống không khớp!", exception.Message);
        }
    }
}