using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class UpdateMedicalSupplyInventoryTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public UpdateMedicalSupplyInventoryTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventory with MedicalSupplyId: 1, ImportQuantity: 100 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(1))
                .Returns(new MedicalSupplyInventory
                {
                    SupplyInventoryId = 1,
                    MedicalSupplyId = 1,
                    ImportQuantity = 100,
                    Quantity = 50,
                    CertificateNumber = "CERT123",
                    BatchNumber = "BATCH123",
                    ManufactureDate = DateTime.Now.AddDays(-30),
                    TransactionDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(365),
                    ReceiverId = 1,
                    TransactionType = true,
                    Note = "Test"
                });
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_ValidDTO_ReturnsTrue()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 80,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Updated Test"
            };

            _mockRepository.Setup(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventory(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_NullDTO_ThrowsException()
        {
            // Arrange
            MedicalSupplyInventoryDTO dto = null;

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventory(dto));
            Assert.Equal("Medical supply inventory is not valid", exception.Message);
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_InvalidSupplyInventoryId_ThrowsException()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                SupplyInventoryId = -1,
                MedicalSupplyId = 1,
                Quantity = 50,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(-1))
                .Returns((MedicalSupplyInventory)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventory(dto));
            Assert.Equal("Medical supply inventory is not exist", exception.Message);
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_QuantityExceedsImportQuantity_ThrowsException()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 200,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.UpdateMedicalSupplyInventory(dto));
            Assert.Equal("Số lượng tồn không hợp lệ", exception.Message);
        }
    }
}