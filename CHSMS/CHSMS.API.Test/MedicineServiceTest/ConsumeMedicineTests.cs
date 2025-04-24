using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumeMedicineTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public ConsumeMedicineTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void ConsumeMedicine_ReturnsConsumptionId()
        {
            // Arrange
            var dto = TestHelper.CreateConsumeMedicineDTO(1, 1, 50);
            _medicineRepositoryMock.Setup(repo => repo.ConsumeMedicineByMedicineId(dto)).Returns(1);

            // Act
            var result = _service.ConsumeMedicine(dto);

            // Assert
            Assert.Equal(1, result);
        }
    }
}
