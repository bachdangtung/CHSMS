using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class ConsumptionDetailTests
    {
        private readonly Mock<IMedicineRepository> _mockRepo;
        private readonly MedicineService _service;

        public ConsumptionDetailTests()
        {
            _mockRepo = new Mock<IMedicineRepository>();
            var mockLogger = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_mockRepo.Object, mockLogger.Object);
        }

        [Fact]
        public void MedicineConsumptionDetail_ValidIdWithNullDates_ReturnsAllConsumptions()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = null;
            DateTime? to = null;

            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = new DateTime(2025, 4, 4) },
                new MedicineConsumption { MedicineConsumptionId = 2, ConsumptionDate = new DateTime(2025, 4, 5) }
            };

            _mockRepo.Setup(x => x.MedicineConsumptionDetail(medicineId, null, null))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(medicineId, null, null), Times.Once);
        }

        [Fact]
        public void MedicineConsumptionDetail_ValidIdWithDateRange_ReturnsFilteredConsumptions()
        {
            // Arrange
            int medicineId = 1;
            DateTime from = new DateTime(2025, 3, 3);
            DateTime to = new DateTime(2025, 5, 5);

            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = new DateTime(2025, 4, 4) }
            };

            _mockRepo.Setup(x => x.MedicineConsumptionDetail(medicineId, from, to))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(medicineId, from, to), Times.Once);
        }

        [Fact]
        public void MedicineConsumptionDetail_InvalidId_ReturnsEmptyList()
        {
            // Arrange
            int medicineId = -1;
            DateTime? from = null;
            DateTime? to = null;

            _mockRepo.Setup(x => x.MedicineConsumptionDetail(medicineId, null, null))
                .Returns(new List<MedicineConsumption>());

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Empty(result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(medicineId, null, null), Times.Once);
        }

        [Fact]
        public void MedicineConsumptionDetail_ValidIdWithFromDateOnly_ReturnsConsumptionsAfterFromDate()
        {
            // Arrange
            int medicineId = 1;
            DateTime from = new DateTime(2025, 3, 3);
            DateTime? to = null;

            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = new DateTime(2025, 4, 4) },
                new MedicineConsumption { MedicineConsumptionId = 2, ConsumptionDate = new DateTime(2025, 5, 5) }
            };

            _mockRepo.Setup(x => x.MedicineConsumptionDetail(medicineId, from, null))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(medicineId, from, null), Times.Once);
        }

        [Fact]
        public void MedicineConsumptionDetail_ValidIdWithToDateOnly_ReturnsConsumptionsBeforeToDate()
        {
            // Arrange
            int medicineId = 1;
            DateTime? from = null;
            DateTime to = new DateTime(2025, 5, 5);

            var expectedConsumptions = new List<MedicineConsumption>
            {
                new MedicineConsumption { MedicineConsumptionId = 1, ConsumptionDate = new DateTime(2025, 4, 4) }
            };

            _mockRepo.Setup(x => x.MedicineConsumptionDetail(medicineId, null, to))
                .Returns(expectedConsumptions);

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Equal(expectedConsumptions, result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(medicineId, null, to), Times.Once);
        }

        [Fact]
        public void MedicineConsumptionDetail_ValidIdWithInvalidDateRange_ReturnsEmptyList()
        {
            // Arrange
            int medicineId = 1;
            DateTime from = new DateTime(2025, 5, 5); // from > to
            DateTime to = new DateTime(2025, 3, 3);

            // Act
            var result = _service.ConsumptionDetail(medicineId, from, to);

            // Assert
            Assert.Empty(result);
            _mockRepo.Verify(x => x.MedicineConsumptionDetail(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
        }
    }
}
