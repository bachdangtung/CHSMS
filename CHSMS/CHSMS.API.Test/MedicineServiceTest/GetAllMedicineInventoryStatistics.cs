using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllMedicineInventoryStatistics
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public GetAllMedicineInventoryStatistics()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void GetAllMedicineInventoryStatistics_ReturnsCorrectStatistics()
        {
            // Arrange
            var expectedStatistics = new List<MedicineInventoryStatistic>
            {
                new MedicineInventoryStatistic
                {
                    MedicineInventoryStatisticsId = 1,
                    MedicineInventoryId = 1,
                    Quantity = 100,
                    ActualQuantity = 95,
                    StatisticDate = DateTime.Now.AddDays(-1),
                    IsUpdate = true
                },
                new MedicineInventoryStatistic
                {
                    MedicineInventoryStatisticsId = 2,
                    MedicineInventoryId = 2,
                    Quantity = 50,
                    ActualQuantity = 50,
                    StatisticDate = DateTime.Now,
                    IsUpdate = false
                }
            };

            _mockRepo.Setup(x => x.GetAllMedicineInventoryStatistics())
                .Returns(expectedStatistics);

            // Act
            var result = _service.GetAllMedicineInventoryStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStatistics.Count, result.Count);
            Assert.Equal(expectedStatistics[0].MedicineInventoryStatisticsId, result[0].MedicineInventoryStatisticsId);
            Assert.Equal(expectedStatistics[0].ActualQuantity, result[0].ActualQuantity);
            Assert.Equal(expectedStatistics[1].IsUpdate, result[1].IsUpdate);
        }

        [Fact]
        public void GetAllMedicineInventoryStatistics_WhenNoStatisticsExist_ReturnsEmptyList()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetAllMedicineInventoryStatistics())
                .Returns(new List<MedicineInventoryStatistic>());

            // Act
            var result = _service.GetAllMedicineInventoryStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
