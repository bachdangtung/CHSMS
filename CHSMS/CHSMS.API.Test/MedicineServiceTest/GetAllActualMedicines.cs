using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllActualMedicines
    {
        private readonly Mock<IMedicineRepository> _mockMedicineRepository;
        private readonly MedicineService _medicineService;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;


        public GetAllActualMedicines()
        {
            _mockMedicineRepository = new Mock<IMedicineRepository>();
            _medicineService = new MedicineService(_mockMedicineRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllActualMedicines_WhenDateIsNull_CallsGetAllMedicine()
        {
            // Arrange
            var expectedMedicines = new List<MedicineDTO>
        {
            new MedicineDTO { MedicineId = 1, MedicineName = "Medicine A", Quantity = 5 }
        };

            _mockMedicineRepository.Setup(repo => repo.GetAllMedicine()).Returns(new List<Medicine>());
            _mockMedicineRepository.Setup(repo => repo.GetMedicineQuantity(It.IsAny<int>())).Returns(5);

            // Act
            var result = _medicineService.GetAllActualMedicines(null);

            // Assert
            _mockMedicineRepository.Verify(repo => repo.GetAllMedicine(), Times.Once);
            _mockMedicineRepository.Verify(repo => repo.GetActualMedicineQuantity(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void GetAllActualMedicines_WhenDateProvided_CalculatesActualQuantity()
        {
            // Arrange
            var testDate = new DateTime(2025, 4, 4);
            var medicines = new List<Medicine>
        {
            new Medicine { MedicineId = 1, MedicineName = "Medicine A" },
            new Medicine { MedicineId = 2, MedicineName = "Medicine B" }
        };

            _mockMedicineRepository.Setup(repo => repo.GetAllMedicine()).Returns(medicines);
            _mockMedicineRepository.Setup(repo => repo.GetActualMedicineQuantity(1, testDate)).Returns(5); // Positive
            _mockMedicineRepository.Setup(repo => repo.GetActualMedicineQuantity(2, testDate)).Returns(-5); // Negative

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            Assert.Single(result); // Only medicine with positive quantity should be included
            Assert.Equal(1, result[0].MedicineId);
            Assert.Equal(5, result[0].Quantity);
            _mockMedicineRepository.Verify(repo => repo.GetActualMedicineQuantity(1, testDate), Times.Once);
            _mockMedicineRepository.Verify(repo => repo.GetActualMedicineQuantity(2, testDate), Times.Once);
        }

        [Fact]
        public void GetAllActualMedicines_WhenAllQuantitiesNegative_ReturnsEmptyList()
        {
            // Arrange
            var testDate = new DateTime(2025, 4, 4);
            var medicines = new List<Medicine>
        {
            new Medicine { MedicineId = 1, MedicineName = "Medicine A" },
            new Medicine { MedicineId = 2, MedicineName = "Medicine B" }
        };

            _mockMedicineRepository.Setup(repo => repo.GetAllMedicine()).Returns(medicines);
            _mockMedicineRepository.Setup(repo => repo.GetActualMedicineQuantity(It.IsAny<int>(), testDate)).Returns(-1);

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAllActualMedicines_WhenConsumptionExists_AdjustsQuantityCorrectly()
        {
            // Arrange
            var testDate = new DateTime(2025, 5, 5);
            var medicines = new List<Medicine>
        {
            new Medicine { MedicineId = 1, MedicineName = "Medicine A" }
        };

            // Initial inventory: 5, consumption: 5 → expected quantity: 0
            _mockMedicineRepository.Setup(repo => repo.GetAllMedicine()).Returns(medicines);
            _mockMedicineRepository.Setup(repo => repo.GetActualMedicineQuantity(1, testDate)).Returns(0);

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result[0].Quantity);
        }

        [Fact]
        public void GetAllActualMedicines_ConvertsMedicineToDTOCorrectly()
        {
            // Arrange
            var testDate = new DateTime(2025, 4, 4);
            var medicine = new Medicine
            {
                MedicineId = 1,
                MedicineName = "Test Medicine",
            };

            _mockMedicineRepository.Setup(repo => repo.GetAllMedicine()).Returns(new List<Medicine> { medicine });
            _mockMedicineRepository.Setup(repo => repo.GetActualMedicineQuantity(1, testDate)).Returns(5);

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(medicine.MedicineId, dto.MedicineId);
            Assert.Equal(5, dto.Quantity);
        }
    }
}
