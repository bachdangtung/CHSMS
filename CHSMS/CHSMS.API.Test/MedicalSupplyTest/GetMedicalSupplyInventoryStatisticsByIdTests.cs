using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyInventoryStatisticsByIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyInventoryStatisticsByIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsById_ReturnsStatistic_WhenExists()
        {
            // Arrange
            int medicalSupplyId = 1;
            var statistic = new MedicalSupplyInventoryStatistic { Msisid = 1, MsinventoryId = 1, Quantity = 50.0 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryStatisticById(medicalSupplyId)).Returns(statistic);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsById(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(50.0, result.Quantity);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryStatisticById(medicalSupplyId), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            int medicalSupplyId = 999;
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryStatisticById(medicalSupplyId)).Returns((MedicalSupplyInventoryStatistic)null);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsById(medicalSupplyId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryStatisticById(medicalSupplyId), Times.Once());
        }
    }
}