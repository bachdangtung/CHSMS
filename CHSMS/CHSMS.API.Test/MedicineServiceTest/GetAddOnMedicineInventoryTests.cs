using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAddOnMedicineInventoryTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public GetAddOnMedicineInventoryTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void GetAddOnMedicineInventory_ValidIdWithNullDates_ReturnsCorrectQuantity()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = null;
            DateTime? to = null;
            double expectedQuantity = 100;

            _mockRepo.Setup(x => x.GetAddOnMedicineInventory(medicineId, from, to))
                .Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMedicineInventory(medicineId, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepo.Verify(x => x.GetAddOnMedicineInventory(medicineId, from, to), Times.Once);
        }

        [Fact]
        public void GetAddOnMedicineInventory_ValidIdWithFromDate_ReturnsCorrectQuantity()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = null;
            double expectedQuantity = 50;

            _mockRepo.Setup(x => x.GetAddOnMedicineInventory(medicineId, from, to))
                .Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMedicineInventory(medicineId, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepo.Verify(x => x.GetAddOnMedicineInventory(medicineId, from, to), Times.Once);
        }

        [Fact]
        public void GetAddOnMedicineInventory_ValidIdWithToDate_ReturnsCorrectQuantity()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = null;
            DateTime? to = new DateTime(2025, 4, 4);
            double expectedQuantity = 75;

            _mockRepo.Setup(x => x.GetAddOnMedicineInventory(medicineId, from, to))
                .Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMedicineInventory(medicineId, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepo.Verify(x => x.GetAddOnMedicineInventory(medicineId, from, to), Times.Once);
        }

        [Fact]
        public void GetAddOnMedicineInventory_ValidIdWithBothDates_ReturnsCorrectQuantity()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = new DateTime(2025, 3, 3);
            DateTime? to = new DateTime(2025, 5, 5);
            double expectedQuantity = 100;

            _mockRepo.Setup(x => x.GetAddOnMedicineInventory(medicineId, from, to))
                .Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMedicineInventory(medicineId, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepo.Verify(x => x.GetAddOnMedicineInventory(medicineId, from, to), Times.Once);
        }

        [Fact]
        public void GetAddOnMedicineInventory_InvalidId_ReturnsZero()
        {
            // Arrange
            int medicineId = -1;
            DateTime? from = null;
            DateTime? to = null;
            double expectedQuantity = 0;

            _mockRepo.Setup(x => x.GetAddOnMedicineInventory(medicineId, from, to))
                .Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMedicineInventory(medicineId, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepo.Verify(x => x.GetAddOnMedicineInventory(medicineId, from, to), Times.Once);
        }
    }
}
