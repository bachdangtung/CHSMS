using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class SearchMedicineByNameTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public SearchMedicineByNameTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void SearchMedicineByName_ReturnsMatchingMedicines()
        {
            // Arrange
            var medicine = TestHelper.CreateMedicine(1);
            medicine.MedicineInventories = new HashSet<MedicineInventory> { TestHelper.CreateMedicineInventory(1, 1) };
            var medicines = new List<Medicine> { medicine };
            _medicineRepositoryMock.Setup(repo => repo.SearchMedicineByName("Test")).Returns(medicines);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(1)).Returns(100);

            // Act
            var result = _service.SearchMedicineByName("Test");

            // Assert
            Assert.Single(result);
            Assert.Equal("TestMedicine", result[0].MedicineName);
            Assert.Equal(100, result[0].Quantity);
        }

        [Fact]
        public void SearchMedicineByName_ReturnsEmptyListWhenNoMatches()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.SearchMedicineByName("Test")).Returns(new List<Medicine>());

            // Act
            var result = _service.SearchMedicineByName("Test");

            // Assert
            Assert.Empty(result);
        }
    }
}
