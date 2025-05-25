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
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetMedicineImportHistoryTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineImportHistory_ReturnsInventories()
        {
            // Arrange
            var inventories = new List<MedicineInventory>
            {
                TestHelper.CreateMedicineInventory(1),
                TestHelper.CreateMedicineInventory(2)
            };
            var fromDate = DateTime.Now.AddDays(-10);
            var toDate = DateTime.Now;
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineImportHistory(fromDate, toDate)).Returns(inventories);

            // Act
            var result = _service.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicineInventoryId);
        }

        [Fact]
        public void GetMedicineImportHistory_ReturnsNullWhenInvalidDates()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(1);
            var toDate = DateTime.Now;

            // Act
            var result = _service.GetMedicineImportHistory(fromDate, toDate);

            // Assert
            Assert.Null(result);
        }
    }
}
