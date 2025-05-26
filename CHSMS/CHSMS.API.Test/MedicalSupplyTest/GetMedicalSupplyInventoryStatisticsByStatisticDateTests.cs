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

            // Setup precondition: MedicalSupplyInventoryStatistic with StatisticDate: 5/5/2025 exists
            var statistic = new MedicalSupplyInventoryStatistic
            {
                Msisid = 1,
                MsinventoryId = 1,
                Quantity = 10,
                ActualQuantity = 10,
                StatisticPerson = 1,
                StatisticDate = new DateTime(2025, 5, 5),
                IsUpdate = false
            };
            _mockRepository.Setup(r => r.GetAllMedicalSupplyInventoryStatistics())
                .Returns(new List<MedicalSupplyInventoryStatistic> { statistic });
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryStatisticsByStatisticDate(
                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns((DateTime from, DateTime to) =>
                    new List<MedicalSupplyInventoryStatistic> { statistic }.FindAll(s =>
                        s.StatisticDate >= from && s.StatisticDate <= to));
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_BothDatesNull_ReturnsAllStatistics()
        {
            // Arrange
            DateTime? from = null;
            DateTime? to = null;

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(new DateTime(2025, 5, 5), result[0].StatisticDate);
            _mockRepository.Verify(r => r.GetAllMedicalSupplyInventoryStatistics(), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_ValidDateRange_ReturnsStatistics()
        {
            // Arrange
            DateTime? from = new DateTime(2025, 4, 4);
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(new DateTime(2025, 5, 5), result[0].StatisticDate);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryStatisticsByStatisticDate(
                new DateTime(2025, 4, 4), new DateTime(2025, 5, 5)), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryStatisticsByStatisticDate_FromAfterToDate_ReturnsNull()
        {
            // Arrange
            DateTime? from = new DateTime(2025, 5, 6);
            DateTime? to = new DateTime(2025, 5, 5);

            // Act
            var result = _service.GetMedicalSupplyInventoryStatisticsByStatisticDate(from, to);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryStatisticsByStatisticDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }
    }
}