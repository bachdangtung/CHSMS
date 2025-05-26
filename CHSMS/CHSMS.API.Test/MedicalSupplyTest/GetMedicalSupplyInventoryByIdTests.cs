using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyInventoryByIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyInventoryByIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventory with SupplyInventoryId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(new MedicalSupplyInventory
                {
                    SupplyInventoryId = 1,
                    MedicalSupplyId = 1,
                    Quantity = 10,
                    CertificateNumber = "CERT123",
                    BatchNumber = "BATCH123",
                    ManufactureDate = DateTime.Now.AddDays(-30),
                    TransactionDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(365),
                    ReceiverId = 1,
                    TransactionType = true,
                    Note = "Test",
                    ImportQuantity = 10
                });
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_ValidId_ReturnsMedicalSupplyInventory()
        {
            // Arrange
            int medicalSupplyInventoryId = 1;

            // Act
            var result = _service.GetMedicalSupplyInventoryById(medicalSupplyInventoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicalSupplyInventoryId, result.SupplyInventoryId);
            Assert.Equal(1, result.MedicalSupplyId);
            Assert.Equal(10, result.Quantity);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryById(medicalSupplyInventoryId), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_InvalidId_ReturnsNull()
        {
            // Arrange
            int medicalSupplyInventoryId = -1;
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(medicalSupplyInventoryId))
                .Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.GetMedicalSupplyInventoryById(medicalSupplyInventoryId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryById(medicalSupplyInventoryId), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_NullId_ThrowsArgumentNullException()
        {
            // Arrange
            int? medicalSupplyInventoryId = null;

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.GetMedicalSupplyInventoryById(medicalSupplyInventoryId));
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryById(It.IsAny<int>()), Times.Never());
        }
    }
}