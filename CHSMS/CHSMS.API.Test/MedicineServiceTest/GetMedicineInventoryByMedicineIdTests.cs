using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetMedicineInventoryByMedicineIdTests
    {
        private readonly MedicineService _medicineService;
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;

        public GetMedicineInventoryByMedicineIdTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _medicineService = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetMedicineInventoryByMedicineId_ValidMedicineId_ReturnsNonEmptyList()
        {
            // Arrange
            int medicineId = 1;
            var medicineInventoryDto = new MedicineInventoryDetailDTO
            {
                MedicineInventoryId = 1,
                MedicineId = 1,
                MedicineName = "Paracetamol",
                Quantity = 100,
                ImportQuantity = 100,
                BatchNumber = "BATCH001",
                ManufacturingDate = new DateTime(2024, 1, 1),
                ExpiryDate = new DateTime(2026, 1, 1),
                TransactionDate = new DateTime(2024, 6, 1),
                CertificateNumber = "CERT001",
                ReceiverId = 1,
                ReceiverName = "John Doe",
                SupplierId = 1,
                SupplierName = "PharmaCorp",
                Note = "Initial import"
            };

            var medicineInventoryDtos = new List<MedicineInventoryDetailDTO> { medicineInventoryDto };

            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryByMedicineId(medicineId))
                .Returns(medicineInventoryDtos);

            // Act
            var result = _medicineService.GetMedicineInventoryByMedicineId(medicineId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result); // Expecting one item in the list
            var dto = result.First();
            Assert.Equal(1, dto.MedicineInventoryId);
            Assert.Equal(1, dto.MedicineId);
            Assert.Equal("Paracetamol", dto.MedicineName);
            Assert.Equal(100, dto.Quantity);
            Assert.Equal("BATCH001", dto.BatchNumber);
            Assert.Equal("CERT001", dto.CertificateNumber);
            Assert.Equal(new DateTime(2024, 1, 1), dto.ManufacturingDate);
            Assert.Equal(new DateTime(2026, 1, 1), dto.ExpiryDate);
            Assert.Equal(new DateTime(2024, 6, 1), dto.TransactionDate);
            Assert.Equal(1, dto.ReceiverId);
            Assert.Equal("John Doe", dto.ReceiverName);
            Assert.Equal(1, dto.SupplierId);
            Assert.Equal("PharmaCorp", dto.SupplierName);
            Assert.Equal("Initial import", dto.Note);
        }

        [Fact]
        public void GetMedicineInventoryByMedicineId_InvalidMedicineId_ReturnsEmptyList()
        {
            // Arrange
            int medicineId = -1;
            _medicineRepositoryMock
                .Setup(repo => repo.GetMedicineInventoryByMedicineId(medicineId))
                .Returns(new List<MedicineInventoryDetailDTO>());

            // Act
            var result = _medicineService.GetMedicineInventoryByMedicineId(medicineId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}

