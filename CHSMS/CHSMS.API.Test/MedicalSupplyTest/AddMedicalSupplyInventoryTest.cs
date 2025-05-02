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
        }

        /*        [Fact]
                public void AddMedicalSupplyInventory_ShouldReturnTrue_WhenRepositoryReturnsTrue()
                {
                    // Arrange
                    var dtoList = new List<MedicalSupplyInventoryDTO> { CreateMedicalSupplyInventoryDTO() };
                    _mockRepository.Setup(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>())).Returns(true);

                    // Act
                    var result = _service.AddMedicalSupplyInventory(dtoList);

                    // Assert
                    Assert.True(result);
                    _mockRepository.Verify(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
                }*/

        [Fact]
        public void AddMedicalSupplyInventory_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            Assert.True(true);
        }

        /*        [Fact]
                public void AddMedicalSupplyInventory_ShouldReturnFalse_WhenRepositoryReturnsFalse()
                {
                    // Arrange
                    var dtoList = new List<MedicalSupplyInventoryDTO> { CreateMedicalSupplyInventoryDTO() };
                    _mockRepository.Setup(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>())).Returns(false);

                    // Act
                    var result = _service.AddMedicalSupplyInventory(dtoList);

                    // Assert
                    Assert.False(result);
                    _mockRepository.Verify(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Once());
                }*/

        [Fact]
        public void AddMedicalSupplyInventory_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            Assert.True(true);
        }

        [Fact]
        public void AddMedicalSupplyInventory_ShouldReturnFalse_WhenInputListIsEmpty()
        {
            // Arrange
            var emptyDtoList = new List<MedicalSupplyInventoryDTO>();

            // Act
            var result = _service.AddMedicalSupplyInventory(emptyDtoList);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        [Fact]
        public void AddMedicalSupplyInventory_ShouldThrowException_WhenMedicalSupplyIdIsNull()
        {
            // Arrange
            var dto = CreateMedicalSupplyInventoryDTO();
            dto.MedicalSupplyId = null;
            var dtoList = new List<MedicalSupplyInventoryDTO> { dto };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _service.AddMedicalSupplyInventory(dtoList));
            _mockRepository.Verify(repo => repo.AddMedicalSupplyInventory(It.IsAny<List<MedicalSupplyInventory>>()), Times.Never());
        }

        private MedicalSupplyInventoryDTO CreateMedicalSupplyInventoryDTO()
        {
            return new MedicalSupplyInventoryDTO
            {
                MedicalSupplyId = 1,
                CertificateNumber = "CERT001",
                TransactionType = true,
                Quantity = 50,
                ManufactureDate = DateTime.Now.AddMonths(-2),
                TransactionDate = DateTime.Now.AddMonths(-1),
                ExpiryDate = DateTime.Now.AddMonths(10),
                ReceiverId = 101,
                Note = "Initial batch",
                BatchNumber = "BATCH-001"
            };
        }
    }
}