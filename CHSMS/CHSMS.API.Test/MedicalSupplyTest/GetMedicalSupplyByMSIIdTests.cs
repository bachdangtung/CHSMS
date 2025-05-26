using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyByMSIIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyByMSIIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupply with MedicalSupplyId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyByMSIID(1))
                .Returns(new MedicalSupply { MedicalSupplyId = 1, MedicalSupplyName = "Test Supply" });
        }

        [Fact]
        public void GetMedicalSupplyByMSIId_ValidMSIId_ReturnsMedicalSupply()
        {
            // Arrange
            int msiId = 1;
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(msiId))
                .Returns(new MedicalSupplyInventory { SupplyInventoryId = msiId, MedicalSupplyId = 1 });

            // Act
            var result = _service.GetMedicalSupplyByMSIId(msiId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicalSupplyId);
            Assert.Equal("Test Supply", result.MedicalSupplyName);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryById(msiId), Times.Once());
            _mockRepository.Verify(r => r.GetMedicalSupplyByMSIID(1), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyByMSIId_InvalidMSIId_ReturnsNull()
        {
            // Arrange
            int msiId = -1;
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryById(msiId))
                .Returns((MedicalSupplyInventory)null);

            // Act
            var result = _service.GetMedicalSupplyByMSIId(msiId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryById(msiId), Times.Once());
            _mockRepository.Verify(r => r.GetMedicalSupplyByMSIID(It.IsAny<int>()), Times.Never());
        }
    }
}