using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumptionHistoryTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;
        private readonly DateTime _testDate = new DateTime(2025, 4, 4);

        public ConsumptionHistoryTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void ConsumptionHistory_BothDatesNull_ReturnsAllRecords()
        {
            // Arrange
            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = _testDate }
            };

            _mockRepo.Setup(x => x.ConsumptionHistory(null, null))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionHistory(null, null);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.ConsumptionHistory(null, null), Times.Once);
        }

        [Fact]
        public void ConsumptionHistory_FromDateOnly_ReturnsRecordsFromDate()
        {
            // Arrange
            var fromDate = new DateTime(2025, 3, 3);
            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = _testDate }
            };

            _mockRepo.Setup(x => x.ConsumptionHistory(fromDate, null))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionHistory(fromDate, null);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.ConsumptionHistory(fromDate, null), Times.Once);
        }

        [Fact]
        public void ConsumptionHistory_ToDateOnly_ReturnsRecordsUpToDate()
        {
            // Arrange
            var toDate = new DateTime(2025, 5, 5);
            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = _testDate }
            };

            _mockRepo.Setup(x => x.ConsumptionHistory(null, toDate))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionHistory(null, toDate);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.ConsumptionHistory(null, toDate), Times.Once);
        }

        [Fact]
        public void ConsumptionHistory_BothDatesProvided_ReturnsRecordsInRange()
        {
            // Arrange
            var fromDate = new DateTime(2025, 3, 3);
            var toDate = new DateTime(2025, 5, 5);
            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = _testDate }
            };

            _mockRepo.Setup(x => x.ConsumptionHistory(fromDate, toDate))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionHistory(fromDate, toDate);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.ConsumptionHistory(fromDate, toDate), Times.Once);
        }

        [Fact]
        public void ConsumptionHistory_NoRecordsFound_ReturnsEmptyList()
        {
            // Arrange
            var fromDate = new DateTime(2025, 3, 3);
            var toDate = new DateTime(2025, 5, 5);

            _mockRepo.Setup(x => x.ConsumptionHistory(fromDate, toDate))
                .Returns(new List<MedicineConsumption>());

            // Act
            var result = _service.ConsumptionHistory(fromDate, toDate);

            // Assert
            Assert.Empty(result);
            _mockRepo.Verify(x => x.ConsumptionHistory(fromDate, toDate), Times.Once);
        }
    }
}
