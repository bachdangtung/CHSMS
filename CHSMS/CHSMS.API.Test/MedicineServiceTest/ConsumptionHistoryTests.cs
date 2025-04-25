using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumptionHistoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public ConsumptionHistoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void ConsumptionHistory_ReturnsConsumptionHistory()
        {
            // Arrange
            var consumptions = new List<MedicineConsumption>
            {
                TestHelper.CreateMedicineConsumption(1, 1, 50),
                TestHelper.CreateMedicineConsumption(2, 2, 25)
            };
            _medicineRepositoryMock.Setup(repo => repo.ConsumptionHistory(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(consumptions);

            // Act
            var result = _service.ConsumptionHistory(DateTime.Now.AddDays(-10), DateTime.Now);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(50, result[0].Amount);
        }

        [Fact]
        public void ConsumptionHistory_ReturnsEmptyListWhenNoHistory()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.ConsumptionHistory(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(new List<MedicineConsumption>());

            // Act
            var result = _service.ConsumptionHistory(DateTime.Now.AddDays(-10), DateTime.Now);

            // Assert
            Assert.Empty(result);
        }
    }
}
