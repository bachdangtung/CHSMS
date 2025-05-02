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
        }

        /*        [Fact]
                public void GetMedicalSupplyByMSIId_ReturnsMedicalSupply_WhenExists()
                {
                    // Arrange
                    int id = 1;
                    var medicalSupply = new MedicalSupply { MedicalSupplyId = 1, MedicalSupplyName = "Supply1" };
                    _mockRepository.Setup(repo => repo.GetMedicalSupplyByMSIID(id)).Returns(medicalSupply);

                    // Act
                    var result = _service.GetMedicalSupplyByMSIId(id);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal("Supply1", result.MedicalSupplyName);
                    _mockRepository.Verify(repo => repo.GetMedicalSupplyByMSIID(id), Times.Once());
                }

                [Fact]
                public void GetMedicalSupplyByMSIId_ReturnsNull_WhenNotExists()
                {
                    // Arrange
                    int id = 999;
                    _mockRepository.Setup(repo => repo.GetMedicalSupplyByMSIID(id)).Returns((MedicalSupply)null);

                    // Act
                    var result = _service.GetMedicalSupplyByMSIId(id);

                    // Assert
                    Assert.Null(result);
                    _mockRepository.Verify(repo => repo.GetMedicalSupplyByMSIID(id), Times.Once());
                }*/
    }
}