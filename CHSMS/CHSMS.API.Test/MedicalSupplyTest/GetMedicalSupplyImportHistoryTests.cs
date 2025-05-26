using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyImportHistoryTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;
        private readonly DateTime _currentDate = new DateTime(2025, 5, 25, 8, 28, 0); // Current date: 25/5/2025 08:28

        public GetMedicalSupplyImportHistoryTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup preconditions: MedicalSupplyInventory records
            var inventory1 = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                TransactionDate = new DateTime(2025, 5, 10)
            };
            var inventory2 = new MedicalSupplyInventory
            {
                SupplyInventoryId = 2,
                TransactionDate = new DateTime(2025, 5, 15)
            };
            _mockRepository.Setup(r => r.GetMedicalSupplyImportHistory(
                new DateTime(2025, 5, 1), new DateTime(2025, 5, 20)))
                .Returns(new List<MedicalSupplyInventory> { inventory1, inventory2 });
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_ValidDateRange_ReturnsInventoryList()
        {
            // Arrange
            DateTime? fromDate = new DateTime(2025, 5, 1);
            DateTime? toDate = new DateTime(2025, 5, 20);

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.SupplyInventoryId == 1);
            Assert.Contains(result, x => x.SupplyInventoryId == 2);
            _mockRepository.Verify(r => r.GetMedicalSupplyImportHistory(
                new DateTime(2025, 5, 1), new DateTime(2025, 5, 20)), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_FromDateAfterToDate_ReturnsNull()
        {
            // Arrange
            DateTime? fromDate = new DateTime(2025, 5, 20);
            DateTime? toDate = new DateTime(2025, 5, 10);

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyImportHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_FutureDateRange_ReturnsNull()
        {
            // Arrange
            DateTime? fromDate = new DateTime(2026, 5, 20);
            DateTime? toDate = new DateTime(2026, 5, 30);
            _mockRepository.Setup(r => r.GetMedicalSupplyImportHistory(
                new DateTime(2026, 5, 20), new DateTime(2026, 5, 30)))
                .Returns(new List<MedicalSupplyInventory>());

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_NullFromDate_UsesMinValue()
        {
            // Arrange
            DateTime? fromDate = null;
            DateTime? toDate = new DateTime(2025, 5, 20);
            _mockRepository.Setup(r => r.GetMedicalSupplyImportHistory(
                DateTime.MinValue, new DateTime(2025, 5, 20)))
                .Returns(new List<MedicalSupplyInventory>
                {
                    new MedicalSupplyInventory { SupplyInventoryId = 1, TransactionDate = new DateTime(2025, 5, 10) },
                    new MedicalSupplyInventory { SupplyInventoryId = 2, TransactionDate = new DateTime(2025, 5, 15) }
                });

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(r => r.GetMedicalSupplyImportHistory(
                DateTime.MinValue, new DateTime(2025, 5, 20)), Times.Once());
        }
    }
}