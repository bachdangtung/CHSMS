using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetExpiryMSITests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetExpiryMSITests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetExpiryMSI_ReturnsNumber_WhenDataExists()
        {
            // Arrange
            int medicalSupplyId = 1;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            double expectedCount = 5;
            _mockRepository.Setup(repo => repo.GetNumberOfExpiredMSI(medicalSupplyId, from, to)).Returns(expectedCount);

            // Act
            var result = _service.GetExpiryMSI(medicalSupplyId, from, to);

            // Assert
            Assert.Equal(expectedCount, result);
            _mockRepository.Verify(repo => repo.GetNumberOfExpiredMSI(medicalSupplyId, from, to), Times.Once());
        }

        [Fact]
        public void GetExpiryMSI_ReturnsZero_WhenNoExpiredItems()
        {
            // Arrange
            int medicalSupplyId = 999;
            DateTime? from = DateTime.Now.AddDays(-30);
            DateTime? to = DateTime.Now;
            _mockRepository.Setup(repo => repo.GetNumberOfExpiredMSI(medicalSupplyId, from, to)).Returns(0);

            // Act
            var result = _service.GetExpiryMSI(medicalSupplyId, from, to);

            // Assert
            Assert.Equal((double)0, result);
            _mockRepository.Verify(repo => repo.GetNumberOfExpiredMSI(medicalSupplyId, from, to), Times.Once());
        }
    }
}