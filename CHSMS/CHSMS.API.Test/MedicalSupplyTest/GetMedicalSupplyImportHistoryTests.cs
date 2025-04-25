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

        public GetMedicalSupplyImportHistoryTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_ReturnsHistory_WhenValidDateRange()
        {
            // Arrange
            DateTime fromDate = DateTime.Now.AddDays(-30);
            DateTime toDate = DateTime.Now;
            var history = GetSampleInventories();
            _mockRepository.Setup(repo => repo.GetMedicalSupplyImportHistory(fromDate, toDate)).Returns(history);

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("BATCH-001", result[0].BatchNumber);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyImportHistory(fromDate, toDate), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_ReturnsNull_WhenInvalidDateRange()
        {
            // Arrange
            DateTime fromDate = DateTime.Now.AddDays(1);
            DateTime toDate = DateTime.Now;

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyImportHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public void GetMedicalSupplyImportHistory_ReturnsEmptyList_WhenNoHistory()
        {
            // Arrange
            DateTime fromDate = DateTime.Now.AddDays(-30);
            DateTime toDate = DateTime.Now;
            var emptyList = new List<MedicalSupplyInventory>();
            _mockRepository.Setup(repo => repo.GetMedicalSupplyImportHistory(fromDate, toDate)).Returns(emptyList);

            // Act
            var result = _service.GetMedicalSupplyImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyImportHistory(fromDate, toDate), Times.Once());
        }

        private List<MedicalSupplyInventory> GetSampleInventories()
        {
            return new List<MedicalSupplyInventory>
            {
                new MedicalSupplyInventory { SupplyInventoryId = 1, MedicalSupplyId = 1, BatchNumber = "BATCH-001", Quantity = 50.0 },
                new MedicalSupplyInventory { SupplyInventoryId = 2, MedicalSupplyId = 1, BatchNumber = "BATCH-002", Quantity = 100.0 }
            };
        }
    }
}