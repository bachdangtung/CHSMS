using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineByIdTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetMedicineByIdTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineById_ReturnsMedicineDTO()
        {
            // Arrange
            var medicine = TestHelper.CreateMedicine(1);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(1)).Returns(medicine);
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineQuantity(1)).Returns(100);

            // Act
            var result = _service.GetMedicineById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicineId);
            Assert.Equal("TestMedicine", result.MedicineName);
            Assert.Equal(100, result.Quantity);
        }

        [Fact]
        public void GetMedicineById_ReturnsNullWhenNotFound()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetMedicine(-1)).Returns((Medicine)null);

            // Act
            var result = _service.GetMedicineById(-1);

            // Assert
            Assert.Null(result);
        }
    }
}
