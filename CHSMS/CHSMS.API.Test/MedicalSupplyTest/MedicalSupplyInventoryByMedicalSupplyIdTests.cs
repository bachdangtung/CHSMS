using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class MedicalSupplyInventoryByMedicalSupplyIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public MedicalSupplyInventoryByMedicalSupplyIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void MedicalSupplyInventoryByMedicalSupplyId_ReturnsDTOList_WhenInventoriesExist()
        {
            // Arrange
            int medicalSupplyId = 1;
            var inventoryList = GetSampleMedicalSupplyInventories();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventory(medicalSupplyId))
                           .Returns(inventoryList);

            // Act
            var result = _service.MedicalSupplyInventoryByMedicalSupplyId(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("BATCH-001", result[0].BatchNumber);
            Assert.Equal(50, result[0].Quantity);
            Assert.Equal("CERT001", result[0].CertificateNumber);
            Assert.Equal("Initial batch", result[0].Note);
            Assert.Equal("BATCH-002", result[1].BatchNumber);
            Assert.Equal(100, result[1].Quantity);
        }

        [Fact]
        public void MedicalSupplyInventoryByMedicalSupplyId_ReturnsEmptyList_WhenNoInventoriesExist()
        {
            // Arrange
            int medicalSupplyId = 999;
            var emptyList = new List<MedicalSupplyInventory>();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplyInventory(medicalSupplyId))
                           .Returns(emptyList);

            // Act
            var result = _service.MedicalSupplyInventoryByMedicalSupplyId(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
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