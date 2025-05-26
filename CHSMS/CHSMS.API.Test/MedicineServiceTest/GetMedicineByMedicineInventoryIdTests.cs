using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineByMedicineInventoryIdTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetMedicineByMedicineInventoryIdTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>(); // Initialize the logger mock
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineByMedicineInventoryId_ReturnsMedicine()
        {
            // Arrange
            var medicine = new Medicine
            {
                MedicineId = 1,
                MedicineName = "TestMedicine",
                // Add other required properties
            };

            _medicineRepositoryMock.Setup(repo => repo.GetMedicineByMedicineInventoryId(1))
                .Returns(medicine);

            // Act
            var result = _service.GetMedicineByMedicineInventoryId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicineId);
            Assert.Equal("TestMedicine", result.MedicineName);
        }

        [Fact]
        public void GetMedicineByMedicineInventoryId_ReturnsNullWhenNotFound()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetMedicineByMedicineInventoryId(1))
                .Returns((Medicine)null);

            // Act
            var result = _service.GetMedicineByMedicineInventoryId(1);

            // Assert
            Assert.Null(result);
        }
    }
}