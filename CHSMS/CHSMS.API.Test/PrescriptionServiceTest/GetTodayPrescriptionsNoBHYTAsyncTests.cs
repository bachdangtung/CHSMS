using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetTodayPrescriptionsNoBHYTAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetTodayPrescriptionsNoBHYTAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetTodayPrescriptionsNoBHYTAsync_NoTodayNonBhytPrescriptions_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                           .ReturnsAsync(new List<Prescription>());

            // Act
            var result = await _service.GetTodayPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTodayPrescriptionsNoBHYTAsync_ValidTodayNonBhytPrescriptions_ReturnsFilteredList()
        {
            // Arrange
            var today = DateTime.Today;
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionId = 1, IssueDate = today, Status = true, IsBhyt = false },
                new Prescription { PrescriptionId = 2, IssueDate = today.AddDays(-1), Status = true, IsBhyt = false }
            };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetTodayPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].PrescriptionId);
        }
        [Fact]
        public async Task GetTodayPrescriptionsNoBHYTAsync_MultipleValidTodayNonBhytPrescriptions_ReturnsAll()
        {
            // Arrange
            var today = DateTime.Today;
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionId = 5, IssueDate = today, Status = true, IsBhyt = false },
                new Prescription { PrescriptionId = 6, IssueDate = today, Status = true, IsBhyt = false },
                new Prescription { PrescriptionId = 7, IssueDate = today, Status = true, IsBhyt = false }
            };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsNoBHYTAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetTodayPrescriptionsNoBHYTAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.PrescriptionId == 5);
            Assert.Contains(result, p => p.PrescriptionId == 6);
            Assert.Contains(result, p => p.PrescriptionId == 7);
        }

    }
}