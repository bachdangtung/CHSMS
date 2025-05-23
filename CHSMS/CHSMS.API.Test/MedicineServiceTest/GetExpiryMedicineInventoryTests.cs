using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetExpiryMedicineInventoryTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public GetExpiryMedicineInventoryTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void GetExpiryMedicineInventory_ValidIdWithNullDates_ReturnsExpiredCount()
        {
            // Arrange
            var medicineId = 1;
            var expectedCount = 2.0; // Two expired items

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(medicineId, null, null))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, null, null);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_ValidIdWithFromDate_ReturnsExpiredCountAfterFromDate()
        {
            // Arrange
            var medicineId = 1;
            var fromDate = new DateTime(2025, 3, 3);
            var expectedCount = 2.0;

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(medicineId, fromDate, null))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, fromDate, null);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_ValidIdWithToDate_ReturnsExpiredCountBeforeToDate()
        {
            // Arrange
            var medicineId = 1;
            var toDate = new DateTime(2025, 4, 4);
            var expectedCount = 1.0; // Only one expired before this date

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(medicineId, null, toDate))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, null, toDate);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_ValidIdWithBothDates_ReturnsExpiredCountInRange()
        {
            // Arrange
            var medicineId = 1;
            var fromDate = new DateTime(2025, 3, 3);
            var toDate = new DateTime(2025, 6, 5);
            var expectedCount = 1.0; // Only one expired in this range

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(medicineId, fromDate, toDate))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, fromDate, toDate);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_InvalidId_ReturnsZero()
        {
            // Arrange
            var invalidMedicineId = -1;
            var expectedCount = 0.0;

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(invalidMedicineId, null, null))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(invalidMedicineId, null, null);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_FromDateAfterToDate_ReturnsZero()
        {
            // Arrange
            var medicineId = 1;
            var fromDate = new DateTime(2025, 6, 5);
            var toDate = new DateTime(2025, 3, 3);
            var expectedCount = 0.0;

            // We don't setup the mock because the method should return 0 without calling repository
            // when dates are invalid

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, fromDate, toDate);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExpiryMedicineInventory_NoExpiredItems_ReturnsZero()
        {
            // Arrange
            var medicineId = 2; // Medicine with no expired items
            var expectedCount = 0.0;

            _mockRepo.Setup(x => x.GetNumberOfExpiredMedicineInventory(medicineId, null, null))
                .Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMedicineInventory(medicineId, null, null);

            // Assert
            Assert.Equal(expectedCount, result);
        }
    }
}
