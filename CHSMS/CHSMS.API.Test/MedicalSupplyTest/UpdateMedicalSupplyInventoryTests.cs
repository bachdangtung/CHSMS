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
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_ReturnsTrue_WhenRepositoryUpdateSucceeds()
        {
            // Arrange
            var dto = CreateMedicalSupplyInventoryDTO();
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                           .Returns(true);

            // Act
            var result = _service.UpdateMedicalSupplyInventory(dto);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.Is<List<MedicalSupplyInventory>>(list =>
                list.Count == 1 &&
                list[0].SupplyInventoryId == dto.SupplyInventoryId &&
                list[0].MedicalSupplyId == dto.MedicalSupplyId &&
                list[0].Quantity == dto.Quantity &&
                list[0].CertificateNumber == dto.CertificateNumber &&
                list[0].Note == dto.Note)), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_ReturnsFalse_WhenRepositoryUpdateFails()
        {
            // Arrange
            var dto = CreateMedicalSupplyInventoryDTO();
            _mockRepository.Setup(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()))
                           .Returns(false);

            // Act
            var result = _service.UpdateMedicalSupplyInventory(dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_ThrowsException_WhenMedicalSupplyIdIsNull()
        {
            // Arrange
            var dto = CreateMedicalSupplyInventoryDTO();
            dto.MedicalSupplyId = null;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.UpdateMedicalSupplyInventory(dto));
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalSupplyInventory_ThrowsException_WhenDTOIsNull()
        {
            // Arrange
            MedicalSupplyInventoryDTO dto = null;

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.UpdateMedicalSupplyInventory(dto));
            _mockRepository.Verify(repo => repo.UpdateMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        private MedicalSupplyInventoryDTO CreateMedicalSupplyInventoryDTO()
        {
            return new MedicalSupplyInventoryDTO
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
                Note = "Updated batch",
                BatchNumber = "BATCH-001"
            };
        }
    }
}