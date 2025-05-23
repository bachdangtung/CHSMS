using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineImportHistoryTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _medicineService;

        public GetMedicineImportHistoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineImportHistory_ValidDateRange_ReturnsMedicineInventories()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 1);
            var toDate = new DateTime(2025, 5, 20);

            var expectedInventories = new List<MedicineInventory>
        {
            new MedicineInventory
            {
                MedicineInventoryId = 1,
                MedicineId = 101,
                TransactionDate = new DateTime(2025, 5, 10),
                Quantity = 100,
                BatchNumber = "BATCH001"
            },
            new MedicineInventory
            {
                MedicineInventoryId = 2,
                MedicineId = 102,
                TransactionDate = new DateTime(2025, 5, 15),
                Quantity = 200,
                BatchNumber = "BATCH002"
            }
        };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineImportHistory(fromDate, toDate))
                .Returns(expectedInventories);

            // Act
            var result = _medicineService.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedInventories, result);
            Assert.Equal(2, result.Count);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineImportHistory(fromDate, toDate), Times.Once());
        }

        [Fact]
        public void GetMedicineImportHistory_FromDateGreaterThanToDate_ReturnsNull()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 20);
            var toDate = new DateTime(2025, 5, 10);

            // Act
            var result = _medicineService.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineImportHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public void GetMedicineImportHistory_FromDateInFuture_ReturnsNull()
        {
            // Arrange
            var fromDate = new DateTime(2026, 5, 20);
            var toDate = new DateTime(2026, 5, 30);


            // Act
            var result = _medicineService.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineImportHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public void GetMedicineImportHistory_EmptyResultFromRepository_ReturnsEmptyList()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 1);
            var toDate = new DateTime(2025, 5, 20);

            var emptyInventories = new List<MedicineInventory>();
            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineImportHistory(fromDate, toDate))
                .Returns(emptyInventories);

            // Act
            var result = _medicineService.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _medicineRepositoryMock.Verify(repo => repo.GetMedicineImportHistory(fromDate, toDate), Times.Once());
        }
    }
}
