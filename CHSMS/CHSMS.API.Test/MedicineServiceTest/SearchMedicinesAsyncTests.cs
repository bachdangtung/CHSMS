using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class SearchMedicinesAsyncTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public SearchMedicinesAsyncTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SearchMedicinesAsync_ReturnsMatchingMedicines()
        {
            // Arrange
            var inventory = TestHelper.CreateMedicineInventory(1, 1, 100, "BATCH001");
            var inventories = new List<MedicineInventory> { inventory };
            _medicineRepositoryMock.Setup(repo => repo.SearchMedicinesAsync(
                null, "TestMedicine", null, null, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(inventories);

            // Act
            var result = await _service.SearchMedicinesAsync(medicineName: "TestMedicine");

            // Assert
            Assert.Single(result);
            Assert.Equal("TestMedicine", result[0].MedicineName);
            Assert.Equal(100, result[0].Quantity);
        }

        [Fact]
        public async Task SearchMedicinesAsync_ReturnsEmptyListWhenNoMatches()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.SearchMedicinesAsync(
                null, "TestMedicine", null, null, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(new List<MedicineInventory>());

            // Act
            var result = await _service.SearchMedicinesAsync(medicineName: "TestMedicine");

            // Assert
            Assert.Empty(result);
        }
    }
}
