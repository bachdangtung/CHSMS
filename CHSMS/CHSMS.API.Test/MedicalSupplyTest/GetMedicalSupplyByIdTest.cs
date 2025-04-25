using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyByIdTest
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyByIdTest()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetMedicalSupplyById_ReturnsDTOList_WhenInventoryExists()
        {
            // Arrange
            int medicalSupplyId = 1;
            var inventoryList = GetSampleMedicalSupplyInventories();

            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryByMSID(medicalSupplyId))
                           .Returns(inventoryList);

            // Act
            var result = _service.GetMedicalSupplyById(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("BATCH-001", result[0].BatchNumber);
            Assert.Equal(50, result[0].Quantity);
            Assert.Equal("CERT001", result[0].CertificateNumber);
        }

        [Fact]
        public void GetMedicalSupplyById_ReturnsNull_WhenInventoryIsNull()
        {
            // Arrange
            int nonExistingId = 999;

            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryByMSID(nonExistingId))
                           .Returns((List<MedicalSupplyInventory>?)null);

            // Act
            var result = _service.GetMedicalSupplyById(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMedicalSupplyById_ReturnsNull_WhenIdNotExistInDatabase()
        {
            // Arrange
            int medicalSupplyId = 5;
            var inventoryList = GetSampleMedicalSupplyInventories();

            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryByMSID(medicalSupplyId))
                           .Returns((List<MedicalSupplyInventory>?)null);

            // Act
            var result = _service.GetMedicalSupplyById(medicalSupplyId);

            // Assert
            Assert.Null(result);
        }

        private List<MedicalSupplyInventory> GetSampleMedicalSupplyInventories()
        {
            return new List<MedicalSupplyInventory>
    {
        new MedicalSupplyInventory
        {
            SupplyInventoryId = 1,
            MedicalSupplyId = 1,
            CertificateNumber = "CERT001",
            TransactionType = true,
            Quantity = 50,
            ManufactureDate = DateTime.Now.AddMonths(-2),
            TransactionDate = DateTime.Now.AddMonths(-1),
            ExpiryDate = DateTime.Now.AddMonths(10),
            ReceiverId = 101,
            Note = "Initial batch",
            BatchNumber = "BATCH-001",
            ImportQuantity = 50
        },
        new MedicalSupplyInventory
        {
            SupplyInventoryId = 2,
            MedicalSupplyId = 1,
            CertificateNumber = "CERT002",
            TransactionType = false,
            Quantity = 100,
            ManufactureDate = DateTime.Now.AddMonths(-3),
            TransactionDate = DateTime.Now.AddMonths(-2),
            ExpiryDate = DateTime.Now.AddMonths(8),
            ReceiverId = 102,
            Note = "Restock",
            BatchNumber = "BATCH-002",
            ImportQuantity = 100
        }
    };
        }


    }
}
