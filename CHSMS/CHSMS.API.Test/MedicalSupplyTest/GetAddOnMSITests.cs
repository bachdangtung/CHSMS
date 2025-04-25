using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetAddOnMSITests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetAddOnMSITests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetAddOnMSI_ReturnsQuantity_WhenDataExists()
        {
            // Arrange
            int id = 1;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            double expectedQuantity = 50.0;
            _mockRepository.Setup(repo => repo.GetAddOnMSI(id, from, to)).Returns(expectedQuantity);

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(expectedQuantity, result);
            _mockRepository.Verify(repo => repo.GetAddOnMSI(id, from, to), Times.Once());
        }

        [Fact]
        public void GetAddOnMSI_ReturnsZero_WhenNoDataExists()
        {
            // Arrange
            int id = 999;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            _mockRepository.Setup(repo => repo.GetAddOnMSI(id, from, to)).Returns(0.0);

            // Act
            var result = _service.GetAddOnMSI(id, from, to);

            // Assert
            Assert.Equal(0.0, result);
            _mockRepository.Verify(repo => repo.GetAddOnMSI(id, from, to), Times.Once());
        }
    }
}