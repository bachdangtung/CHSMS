using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class AddMedicalSupplyInventoryTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public AddMedicalSupplyInventoryTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupply with MedicalSupplyId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(1))
                .Returns(new List<MedicalSupplyInventory> { new MedicalSupplyInventory { MedicalSupplyId = 1 } });
        }

        [Fact]
        public void AddMedicalSupplyInventory_ValidDTO_ReturnsTrue()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                MedicalSupplyId = 1,
                Quantity = 10,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };
            var dtos = new List<MedicalSupplyInventoryDTO> { dto };

            _mockRepository.Setup(r => r.isExistMedicalSupplyInventory(1, "BATCH123", "CERT123"))
                .Returns(false);
            _mockRepository.Setup(r => r.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                .Returns(true);

            // Act
            var result = _service.AddMedicalSupplyInventory(dtos);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
        }

        [Fact]
        public void AddMedicalSupplyInventory_NullDTOList_ReturnsFalse()
        {
            // Arrange
            List<MedicalSupplyInventoryDTO> dtos = null;

            // Act
            var result = _service.AddMedicalSupplyInventory(dtos);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddMedicalSupplyInventory_EmptyDTOList_ReturnsFalse()
        {
            // Arrange
            var dtos = new List<MedicalSupplyInventoryDTO>();

            // Act
            var result = _service.AddMedicalSupplyInventory(dtos);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddMedicalSupplyInventory_InvalidMedicalSupplyId_ThrowsException()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                MedicalSupplyId = -1,
                Quantity = 10,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };
            var dtos = new List<MedicalSupplyInventoryDTO> { dto };

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(-1))
                .Returns((List<MedicalSupplyInventory>)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.AddMedicalSupplyInventory(dtos));
            Assert.Equal("Vật tư với ID -1 không tồn tại", exception.Message);
        }

        [Fact]
        public void AddMedicalSupplyInventory_DuplicateEntry_ThrowsException()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                MedicalSupplyId = 1,
                Quantity = 10,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };
            var dtos = new List<MedicalSupplyInventoryDTO> { dto };

            _mockRepository.Setup(r => r.isExistMedicalSupplyInventory(1, "BATCH123", "CERT123"))
                .Returns(true);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.AddMedicalSupplyInventory(dtos));
            Assert.Equal("Vật tư đã tồn tại trong kho", exception.Message);
        }

        [Fact]
        public void AddMedicalSupplyInventory_NegativeQuantity_ThrowsException()
        {
            // Arrange
            var dto = new MedicalSupplyInventoryDTO
            {
                MedicalSupplyId = 1,
                Quantity = -1,
                CertificateNumber = "CERT123",
                BatchNumber = "BATCH123",
                ManufactureDate = DateTime.Now.AddDays(-30),
                TransactionDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(365),
                ReceiverId = 1,
                TransactionType = true,
                Note = "Test"
            };
            var dtos = new List<MedicalSupplyInventoryDTO> { dto };

            _mockRepository.Setup(r => r.isExistMedicalSupplyInventory(1, "BATCH123", "CERT123"))
                .Returns(false);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.AddMedicalSupplyInventory(dtos));
            Assert.Equal("Số lượng không hợp lệ", exception.Message);
        }
    }
}