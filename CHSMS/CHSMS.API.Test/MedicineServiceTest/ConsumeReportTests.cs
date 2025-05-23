using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumeReportTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public ConsumeReportTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void ConsumeReport_BothDatesNull_ReturnsAllConsumptions()
        {
            // Arrange
            var testDate = new DateTime(2025, 5, 5);
            var medicine = new Medicine { MedicineId = 1, MedicineName = "Test Medicine" };
            var expectedDict = new Dictionary<Medicine, double>
            {
                { medicine, 10.0 }
            };

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(null, null))
                .Returns(expectedDict);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(1))
                .Returns(5.0);

            // Act
            var result = _service.ConsumeReport(null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Medicine", result.Keys.First().MedicineName);
            Assert.Equal(10.0, result.Values.First());
            Assert.Equal(5.0, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_FromDateOnly_ReturnsConsumptionsFromDate()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 5);
            var medicine = new Medicine { MedicineId = 1, MedicineName = "Test Medicine" };
            var expectedDict = new Dictionary<Medicine, double>
            {
                { medicine, 5.0 }
            };

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(fromDate, null))
                .Returns(expectedDict);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(1))
                .Returns(3.0);

            // Act
            var result = _service.ConsumeReport(fromDate, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Medicine", result.Keys.First().MedicineName);
            Assert.Equal(5.0, result.Values.First());
            Assert.Equal(3.0, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_ToDateOnly_ReturnsConsumptionsUpToDate()
        {
            // Arrange
            var toDate = new DateTime(2025, 5, 10);
            var medicine = new Medicine { MedicineId = 1, MedicineName = "Test Medicine" };
            var expectedDict = new Dictionary<Medicine, double>
            {
                { medicine, 7.0 }
            };

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(null, toDate))
                .Returns(expectedDict);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(1))
                .Returns(2.0);

            // Act
            var result = _service.ConsumeReport(null, toDate);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Medicine", result.Keys.First().MedicineName);
            Assert.Equal(7.0, result.Values.First());
            Assert.Equal(2.0, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_BothDatesProvided_ReturnsConsumptionsInDateRange()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 5);
            var toDate = new DateTime(2025, 5, 10);
            var medicine = new Medicine { MedicineId = 1, MedicineName = "Test Medicine" };
            var expectedDict = new Dictionary<Medicine, double>
            {
                { medicine, 3.0 }
            };

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(fromDate, toDate))
                .Returns(expectedDict);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(1))
                .Returns(4.0);

            // Act
            var result = _service.ConsumeReport(fromDate, toDate);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Medicine", result.Keys.First().MedicineName);
            Assert.Equal(3.0, result.Values.First());
            Assert.Equal(4.0, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_NoConsumptionsInDateRange_ReturnsEmptyDictionary()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 5);
            var toDate = new DateTime(2025, 5, 10);
            var expectedDict = new Dictionary<Medicine, double>();

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(fromDate, toDate))
                .Returns(expectedDict);

            // Act
            var result = _service.ConsumeReport(fromDate, toDate);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ConsumeReport_FromDateAfterToDate_ReturnsEmptyDictionary()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 10);
            var toDate = new DateTime(2025, 5, 5);
            var expectedDict = new Dictionary<Medicine, double>();

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(fromDate, toDate))
                .Returns(expectedDict);

            // Act
            var result = _service.ConsumeReport(fromDate, toDate);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ConsumeReport_MultipleMedicines_ReturnsCorrectReport()
        {
            // Arrange
            var medicine1 = new Medicine { MedicineId = 1, MedicineName = "Medicine A" };
            var medicine2 = new Medicine { MedicineId = 2, MedicineName = "Medicine B" };
            var expectedDict = new Dictionary<Medicine, double>
            {
                { medicine1, 5.0 },
                { medicine2, 3.0 }
            };

            _mockRepo.Setup(x => x.GetAllMedicineConsumeReport(null, null))
                .Returns(expectedDict);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(1))
                .Returns(10.0);
            _mockRepo.Setup(x => x.GetMedicineQuantityById(2))
                .Returns(7.0);

            // Act
            var result = _service.ConsumeReport(null, null);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Key.MedicineName == "Medicine A" && x.Value == 5.0 && x.Key.Quantity == 10.0);
            Assert.Contains(result, x => x.Key.MedicineName == "Medicine B" && x.Value == 3.0 && x.Key.Quantity == 7.0);
        }
    }
}
