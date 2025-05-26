using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetMedicalSupplyByIdTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetMedicalSupplyByIdTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupply with MedicalSupplyId: 1 exists
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(1))
                .Returns(new List<MedicalSupplyInventory>
                {
                    new MedicalSupplyInventory
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
                        Note = "Test inventory"
                    }
                });
        }

        [Fact]
        public void GetMedicalSupplyById_ValidId_ReturnsInventoryDTOList()
        {
            // Arrange
            int medicalSupplyId = 1;

            // Act
            var result = _service.GetMedicalSupplyById(medicalSupplyId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<MedicalSupplyInventoryDTO>>(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].SupplyInventoryId);
            Assert.Equal(1, result[0].MedicalSupplyId);
            Assert.Equal(10, result[0].Quantity);
            Assert.Equal("CERT123", result[0].CertificateNumber);
            Assert.Equal("BATCH123", result[0].BatchNumber);
            Assert.Equal("Test inventory", result[0].Note);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryByMSID(medicalSupplyId), Times.Once());
        }

        [Fact]
        public void GetMedicalSupplyById_InvalidId_ReturnsNull()
        {
            // Arrange
            int medicalSupplyId = -1;
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(-1))
                .Returns((List<MedicalSupplyInventory>)null);

            // Act
            var result = _service.GetMedicalSupplyById(medicalSupplyId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetMedicalSupplyInventoryByMSID(medicalSupplyId), Times.Once());
        }
    }
}