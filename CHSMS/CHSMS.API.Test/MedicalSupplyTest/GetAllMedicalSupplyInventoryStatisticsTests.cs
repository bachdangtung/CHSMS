using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetAllMedicalSupplyInventoryStatisticsTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetAllMedicalSupplyInventoryStatisticsTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetAllMedicalSupplyInventoryStatistics_ReturnsStatistics_WhenDataExists()
        {
            // Arrange
            var statistics = GetSampleStatistics();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventoryStatistics()).Returns(statistics);

            // Act
            var result = _service.GetAllMedicalSupplyInventoryStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(50.0, result[0].Quantity);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyInventoryStatistics(), Times.Once());
        }

        [Fact]
        public void GetAllMedicalSupplyInventoryStatistics_ReturnsEmptyList_WhenNoData()
        {
            // Arrange
            var emptyList = new List<MedicalSupplyInventoryStatistic>();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventoryStatistics()).Returns(emptyList);

            // Act
            var result = _service.GetAllMedicalSupplyInventoryStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyInventoryStatistics(), Times.Once());
        }

        [Fact]
        public void GetAllMedicalSupplyInventoryStatistics_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventoryStatistics()).Returns((List<MedicalSupplyInventoryStatistic>)null);

            // Act
            var result = _service.GetAllMedicalSupplyInventoryStatistics();

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyInventoryStatistics(), Times.Once());
        }

        private List<MedicalSupplyInventoryStatistic> GetSampleStatistics()
        {
            return new List<MedicalSupplyInventoryStatistic>
            {
                new MedicalSupplyInventoryStatistic { Msisid = 1, MsinventoryId = 1, Quantity = 50.0, ActualQuantity = 48.0, StatisticPerson = 101 },
                new MedicalSupplyInventoryStatistic { Msisid = 2, MsinventoryId = 2, Quantity = 100.0, ActualQuantity = 95.0, StatisticPerson = 102 }
            };
        }
    }
}