using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllMedicineTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetAllMedicineTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllMedicine_ReturnsMedicinesWithPositiveQuantity()
        {
            // Arrange
            var medicines = new List<Medicine>
            {
                TestHelper.CreateMedicine(1),
                TestHelper.CreateMedicine(2)
            };
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicine()).Returns(medicines);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(1)).Returns(100);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(2)).Returns(200);

            // Act
            var result = _service.GetAllMedicine();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(100, result[0].Quantity);
            Assert.Equal(200, result[1].Quantity);
            Assert.Equal("TestMedicine", result[0].MedicineName);
        }

        [Fact]
        public void GetAllMedicine_SkipsMedicinesWithNegativeQuantity()
        {
            // Arrange
            var medicines = new List<Medicine>
            {
                TestHelper.CreateMedicine(1),
                TestHelper.CreateMedicine(2)
            };
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicine()).Returns(medicines);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(1)).Returns(100);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(2)).Returns(-10);

            // Act
            var result = _service.GetAllMedicine();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MedicineId);
            Assert.Equal(100, result[0].Quantity);
        }

        [Fact]
        public void GetAllMedicine_ReturnsEmptyListWhenNoMedicines()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetAllMedicine()).Returns(new List<Medicine>());

            // Act
            var result = _service.GetAllMedicine();

            // Assert
            Assert.Empty(result);
        }
    }
}
