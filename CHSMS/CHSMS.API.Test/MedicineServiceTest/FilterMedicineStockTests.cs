using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class FilterMedicineStockTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public FilterMedicineStockTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void FilterMedicineStock_ReturnsFilteredMedicines()
        {
            // Arrange
            var filter = new MedicineInventoryFilter();
            var medicines = new List<MedicineDTO> { TestHelper.CreateMedicineDTO(1, "TestMedicine", 100) };
            _medicineRepositoryMock.Setup(repo => repo.GetFilteredMedicineInventory(It.IsAny<MedicineInventoryFilter>())).Returns(medicines);

            // Act
            var result = _service.FilterMedicineStock(filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("TestMedicine", result[0].MedicineName);
            Assert.Equal(100, result[0].Quantity);
        }

        [Fact]
        public void FilterMedicineStock_ReturnsEmptyListWhenNoMatches()
        {
            // Arrange
            var filter = new MedicineInventoryFilter();
            _medicineRepositoryMock.Setup(repo => repo.GetFilteredMedicineInventory(It.IsAny<MedicineInventoryFilter>())).Returns(new List<MedicineDTO>());

            // Act
            var result = _service.FilterMedicineStock(filter);

            // Assert
            Assert.Empty(result);
        }
    }
}
