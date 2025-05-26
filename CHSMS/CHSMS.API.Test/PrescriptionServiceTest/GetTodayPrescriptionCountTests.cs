using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetTodayPrescriptionCountTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetTodayPrescriptionCountTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public void GetTodayPrescriptionCount_NoPrescriptions_ReturnsZero()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CountTodayPrescriptions()).Returns(0);

            // Act
            var result = _service.GetTodayPrescriptionCount();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetTodayPrescriptionCount_ValidPrescriptions_ReturnsCount()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CountTodayPrescriptions()).Returns(5);

            // Act
            var result = _service.GetTodayPrescriptionCount();

            // Assert
            Assert.Equal(5, result);
        }
    }
}