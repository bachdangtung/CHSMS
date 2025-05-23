using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineInventoryStatisticsByStatisticDate
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public GetMedicineInventoryStatisticsByStatisticDate()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void GetMedicineInventoryStatisticsByStatisticDate_BothDatesNull_ReturnsAllStatistics()
        {
            // Arrange
            var expectedStats = new List<MedicineInventoryStatistic>
            {
                new() { StatisticDate = new DateTime(2025, 5, 5) },
                new() { StatisticDate = new DateTime(2025, 4, 1) },
                new() { StatisticDate = new DateTime(2025, 6, 10) }
            };

            _mockRepo.Setup(x => x.GetAllMedicineInventoryStatistics())
                .Returns(expectedStats);

            // Act
            var result = _service.GetMedicineInventoryStatisticsByStatisticDate(null, null);

            // Assert
            Assert.Equal(expectedStats, result);
        }

        [Fact]
        public void GetMedicineInventoryStatisticsByStatisticDate_FromDateAfterCurrentDate_ReturnsNull()
        {
            // Arrange
            var fromDate = new DateTime(2026, 12, 12);
            var toDate = new DateTime(2025, 5, 5);

            // Act
            var result = _service.GetMedicineInventoryStatisticsByStatisticDate(fromDate, toDate);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMedicineInventoryStatisticsByStatisticDate_ValidDateRange_ReturnsFilteredStatistics()
        {
            // Arrange
            var fromDate = new DateTime(2025, 4, 4);
            var toDate = new DateTime(2025, 5, 5);

            var allStats = new List<MedicineInventoryStatistic>
            {
                new() { StatisticDate = new DateTime(2025, 5, 5) },
                new() { StatisticDate = new DateTime(2025, 4, 1) },
                new() { StatisticDate = new DateTime(2025, 4, 15) },
                new() { StatisticDate = new DateTime(2025, 6, 10) }
            };

            var expectedStats = new List<MedicineInventoryStatistic>
            {
                allStats[0], // 5/5/2025
                allStats[2]  // 4/15/2025
            };

            _mockRepo.Setup(x => x.GetMedicineInventoryStatisticsByStatisticDate(fromDate, toDate))
                .Returns(expectedStats);

            // Act
            var result = _service.GetMedicineInventoryStatisticsByStatisticDate(fromDate, toDate);

            // Assert
            Assert.Equal(expectedStats, result);
        }
    }
}
