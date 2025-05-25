using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetTodayPrescriptionsAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetTodayPrescriptionsAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetTodayPrescriptionsAsync_NoTodayBhytPrescriptions_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(new List<Prescription>());

            // Act
            var result = await _service.GetTodayPrescriptionsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTodayPrescriptionsAsync_ValidTodayBhytPrescriptions_ReturnsFilteredList()
        {
            // Arrange
            var today = DateTime.Today;
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionId = 1, IssueDate = today, Status = true, IsBhyt = true },
                new Prescription { PrescriptionId = 2, IssueDate = today.AddDays(-1), Status = true, IsBhyt = true }
            };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetTodayPrescriptionsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].PrescriptionId);
        }
        [Fact]
        public async Task GetTodayPrescriptionsAsync_TodayButNotBhyt_ReturnsEmptyList()
        {
            // Arrange
            var today = DateTime.Today;
            var prescriptions = new List<Prescription>
    {
        new Prescription { PrescriptionId = 3, IssueDate = today, Status = true, IsBhyt = false }
    };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetTodayPrescriptionsAsync();

            // Assert
            Assert.Empty(result);
        }

        

        
    }
}