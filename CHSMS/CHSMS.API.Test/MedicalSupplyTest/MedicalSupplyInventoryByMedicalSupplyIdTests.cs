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

            // Setup precondition: MedicalSupply with MedicalSupplyId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyByID(1))
                .Returns(new MedicalSupply { MedicalSupplyId = 1, MedicalSupplyName = "Test Supply" });
        }

        [Fact]
        public void MedicalSupplyInventoryByMedicalSupplyId_ValidId_ReturnsDTOList()
        {
            // Arrange
            int medicalSupplyId = 1;
            var inventory = new List<MedicalSupplyInventory>
            {
                new MedicalSupplyInventory
                {
                    SupplyInventoryId = 1,
                    MedicalSupplyId = medicalSupplyId,
                    Quantity = 10,
                    CertificateNumber = "CERT123",
                    BatchNumber = "BATCH123",
                    ManufactureDate = DateTime.Now.AddDays(-30),
                    TransactionDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(365),
                    ReceiverId = 1,
                    TransactionType = true,
                    Note = "Test Inventory",
                    ImportQuantity = 10
                }
            };

            _mockRepository.Setup(r => r.GetAllMedicalSupplyInventory(medicalSupplyId))
                .Returns(inventory);

            // Act
            var result = _service.MedicalSupplyInventoryByMedicalSupplyId(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(1, dto.SupplyInventoryId);
            Assert.Equal(medicalSupplyId, dto.MedicalSupplyId);
            Assert.Equal(10, dto.Quantity);
            Assert.Equal("CERT123", dto.CertificateNumber);
            Assert.Equal("BATCH123", dto.BatchNumber);
            Assert.Equal("Test Inventory", dto.Note);
            _mockRepository.Verify(r => r.GetAllMedicalSupplyInventory(medicalSupplyId), Times.Once());
        }

        [Fact]
        public void MedicalSupplyInventoryByMedicalSupplyId_InvalidId_ReturnsEmptyList()
        {
            // Arrange
            int medicalSupplyId = -1;
            _mockRepository.Setup(r => r.GetAllMedicalSupplyInventory(medicalSupplyId))
                .Returns(new List<MedicalSupplyInventory>());

            // Act
            var result = _service.MedicalSupplyInventoryByMedicalSupplyId(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.GetAllMedicalSupplyInventory(medicalSupplyId), Times.Once());
        }
    }
}