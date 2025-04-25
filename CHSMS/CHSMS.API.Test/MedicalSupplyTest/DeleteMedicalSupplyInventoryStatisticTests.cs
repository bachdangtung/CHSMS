using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class DeleteMedicalSupplyInventoryStatisticTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public DeleteMedicalSupplyInventoryStatisticTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void DeleteMedicalSupplyInventoryStatistic_ReturnsTrue_WhenStatisticExists()
        {
            // Arrange
            int statisticId = 1;
            var statistic = new MedicalSupplyInventoryStatistic { Msisid = 1, MsinventoryId = 1 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryStatisticById(statisticId)).Returns(statistic);
            _mockRepository.Setup(repo => repo.DeleteMedicalSupplyInventoryStatistic(statistic)).Returns(true);

            // Act
            var result = _service.DeleteMedicalSupplyInventoryStatistic(statisticId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.DeleteMedicalSupplyInventoryStatistic(statistic), Times.Once());
        }

        [Fact]
        public void DeleteMedicalSupplyInventoryStatistic_ReturnsFalse_WhenStatisticNotFound()
        {
            // Arrange
            int statisticId = 999;
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryStatisticById(statisticId)).Returns((MedicalSupplyInventoryStatistic)null);

            // Act
            var result = _service.DeleteMedicalSupplyInventoryStatistic(statisticId);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.DeleteMedicalSupplyInventoryStatistic(It.IsAny<MedicalSupplyInventoryStatistic>()), Times.Never());
        }
    }
}