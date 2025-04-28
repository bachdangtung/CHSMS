using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumeReportTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public ConsumeReportTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void ConsumeReport_ReturnsReport()
        {
            // Arrange
            var medicine = TestHelper.CreateMedicine(1);
            var dict = new Dictionary<Medicine, double> { { medicine, 50.0 } };
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicineConsumeReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(dict);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantityById(1)).Returns(100);

            // Act
            var result = _service.ConsumeReport(DateTime.Now.AddDays(-10), DateTime.Now);

            // Assert
            Assert.Single(result);
            Assert.Equal("TestMedicine", result.Keys.First().MedicineName);
            Assert.Equal(50.0, result.Values.First());
            Assert.Equal(100, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_NoInputDate_ReturnsReport()
        {
            // Arrange
            var medicine = TestHelper.CreateMedicine(1);
            var dict = new Dictionary<Medicine, double> { { medicine, 50.0 } };
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicineConsumeReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(dict);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantityById(1)).Returns(100);

            // Act
            var result = _service.ConsumeReport(null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("TestMedicine", result.Keys.First().MedicineName);
            Assert.Equal(50.0, result.Values.First());
            Assert.Equal(100, result.Keys.First().Quantity);
        }

        [Fact]
        public void ConsumeReport_ReturnsEmptyWhenNoData()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicineConsumeReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(new Dictionary<Medicine, double>());

            // Act
            var result = _service.ConsumeReport(DateTime.Now.AddDays(-10), DateTime.Now);

            // Assert
            Assert.Empty(result);
        }
    }
}
