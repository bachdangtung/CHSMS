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
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_ReturnsInventory_WhenExists()
        {
            // Arrange
            int? id = 1;
            var inventory = new MedicalSupplyInventory { SupplyInventoryId = 1, MedicalSupplyId = 1, Quantity = 50.0 };
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(1)).Returns(inventory);

            // Act
            var result = _service.GetMedicalSupplyInventoryById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(50.0, result.Quantity);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(1), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            int? id = 999;
            _mockRepository.Setup(repo => repo.GetMedicalSupplyInventoryById(999)).Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.GetMedicalSupplyInventoryById(id);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(999), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyInventoryById_ThrowsException_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _service.GetMedicalSupplyInventoryById(id));
            _mockRepository.Verify(repo => repo.GetMedicalSupplyInventoryById(It.IsAny<int>()), Times.Never());
        }
    }
}