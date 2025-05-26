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
        private readonly Mock<IMedicineRepository> _mockMedicineRepo;
        private readonly MedicineService _medicineService;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;


        public GetAllActualMedicines()
        {
            _mockMedicineRepo = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>(); // Initialize the logger mock
            _medicineService = new MedicineService(_mockMedicineRepo.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllActualMedicines_WhenDateIsNull_ReturnsAllMedicines()
        {
            // Arrange
            var expectedMedicines = new List<MedicineDTO>
        {
            new MedicineDTO { MedicineId = 1, MedicineName = "Medicine1", Quantity = 10 },
            new MedicineDTO { MedicineId = 2, MedicineName = "Medicine2", Quantity = 20 }
        };

            _mockMedicineRepo.Setup(repo => repo.GetAllMedicine())
                .Returns(new List<Medicine>
                {
                new Medicine { MedicineId = 1, MedicineName = "Medicine1" },
                new Medicine { MedicineId = 2, MedicineName = "Medicine2" }
                });

            // Assuming ConvertToMedicineDTO is a method in the service that converts Medicine to MedicineDTO
            // If it's in a separate class, you'll need to mock that as well

            // Act
            var result = _medicineService.GetAllActualMedicines(null);

            // Assert
            Assert.Equal(expectedMedicines.Count, result.Count);
            Assert.Equal(expectedMedicines[0].MedicineId, result[0].MedicineId);
            Assert.Equal(expectedMedicines[1].MedicineId, result[1].MedicineId);
        }

        [Fact]
        public void GetAllActualMedicines_WithSpecificDate_ReturnsActualQuantities()
        {
            // Arrange
            var testDate = new DateTime(2025, 4, 4);
            var medicines = new List<Medicine>
        {
            new Medicine { MedicineId = 1, MedicineName = "Medicine1" },
            new Medicine { MedicineId = 2, MedicineName = "Medicine2" }
        };

            _mockMedicineRepo.Setup(repo => repo.GetAllMedicine()).Returns(medicines);

            // Set up mock for GetActualMedicineQuantity based on your preconditions
            _mockMedicineRepo.Setup(repo => repo.GetActualMedicineQuantity(1, testDate)).Returns(5);
            _mockMedicineRepo.Setup(repo => repo.GetActualMedicineQuantity(2, testDate)).Returns(-5);

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            Assert.Equal(2, result.Count);

            var medicine1 = result.Find(m => m.MedicineId == 1);
            Assert.NotNull(medicine1);
            Assert.Equal(5, medicine1.Quantity);

            var medicine2 = result.Find(m => m.MedicineId == 2);
            Assert.NotNull(medicine2);
            Assert.Equal(-5, medicine2.Quantity);
        }

        [Fact]
        public void GetAllActualMedicines_WithLaterDate_ConsidersConsumption()
        {
            // Arrange
            var testDate = new DateTime(2025, 5, 6); // After consumption date in your preconditions
            var medicines = new List<Medicine>
        {
            new Medicine { MedicineId = 1, MedicineName = "Medicine1" }
        };

            _mockMedicineRepo.Setup(repo => repo.GetAllMedicine()).Returns(medicines);

            // Based on your preconditions:
            // Initial quantity: 5
            // Consumption on 5/5/2025: -5
            // Expected quantity after consumption: 0
            _mockMedicineRepo.Setup(repo => repo.GetActualMedicineQuantity(1, testDate)).Returns(0);

            // Act
            var result = _medicineService.GetAllActualMedicines(testDate);

            // Assert
            var medicine = result.Find(m => m.MedicineId == 1);
            Assert.NotNull(medicine);
            Assert.Equal(0, medicine.Quantity);
        }

        [Fact]
        public void GetAllActualMedicines_WhenDatabaseIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            _mockMedicineRepo.Setup(repo => repo.GetAllMedicine())
                .Returns(new List<Medicine>()); // Return empty list

            // No need to setup GetActualMedicineQuantity since there are no medicines to query

            // Act
            var result = _medicineService.GetAllActualMedicines(null); // Date doesn't matter in this case

            // Assert
            Assert.NotNull(result); // Should return an empty list, not null
            Assert.Empty(result); // Verify the list is empty
        }
    }
}
