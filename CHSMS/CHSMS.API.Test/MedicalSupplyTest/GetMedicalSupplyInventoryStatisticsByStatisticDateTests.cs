using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyInventoryStatisticsByStatisticDateTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyInventoryStatisticsByStatisticDateTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_ReturnsStatistics_WhenValidDateRange()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            var statistics = GetSampleStatistics();
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryStatisticsByStatisticDate(from.Value, to.Value)).Returns(statistics);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(50.0, result[0].Quantity);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryStatisticsByStatisticDate(from.Value, to.Value), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_ReturnsAllStatistics_WhenDatesAreNull()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = null;
            var statistics = GetSampleStatistics();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventoryStatistics()).Returns(statistics);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplyInventoryStatistics(), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_ReturnsNull_WhenInvalidDateRange()
        {
            // Arrange
            DateTime? from = DateTime.Now.AddDays(1);
            DateTime? to = DateTime.Now;

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryStatisticsByStatisticDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
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