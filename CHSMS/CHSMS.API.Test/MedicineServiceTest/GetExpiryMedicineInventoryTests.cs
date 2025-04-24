using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetExpiryMedicineInventoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetExpiryMedicineInventoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetExpiryMedicineInventory_ReturnsExpiredCount()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetNumberOfExpiredMedicineInventory(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(5);

            // Act
            var result = _service.GetExpiryMedicineInventory(1, DateTime.Now.AddDays(-10), DateTime.Now);

            // Assert
            Assert.Equal(5, result);
        }
    }
}
