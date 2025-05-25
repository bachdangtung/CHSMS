using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetAllPrescriptionsNoBHYTAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetAllPrescriptionsNoBHYTAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllPrescriptionsNoBHYTAsync_NoNonBhytPrescriptions_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                           .ReturnsAsync(new List<Prescription>());

            // Act
            var result = await _service.GetAllPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPrescriptionsNoBHYTAsync_ValidNonBhytPrescriptions_ReturnsFilteredList()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now, Status = true, IsBhyt = false },
                new Prescription { PrescriptionId = 2, IssueDate = DateTime.Now, Status = false, IsBhyt = true }
            };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetAllPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].PrescriptionId);
        }
    }
}