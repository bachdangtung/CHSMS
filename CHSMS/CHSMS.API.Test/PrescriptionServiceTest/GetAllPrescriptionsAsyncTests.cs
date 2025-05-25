using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetAllPrescriptionsAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetAllPrescriptionsAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllPrescriptionsAsync_NoBhytPrescriptions_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(new List<Prescription>());

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPrescriptionsAsync_ValidBhytPrescriptions_ReturnsFilteredList()
        {
            // Arrange
            var prescriptions = new List<Prescription>
            {
                new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now, Status = true, IsBhyt = true },
                new Prescription { PrescriptionId = 2, IssueDate = DateTime.Now, Status = false, IsBhyt = false }
            };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].PrescriptionId);
        }
        [Fact]
        public async Task GetAllPrescriptionsAsync_AllNonBhytPrescriptions_ReturnsEmptyList()
        {
            // Arrange
            var prescriptions = new List<Prescription>
    {
        new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now, Status = true, IsBhyt = false },
        new Prescription { PrescriptionId = 2, IssueDate = DateTime.Now, Status = true, IsBhyt = false }
    };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPrescriptionsAsync_BhytWithFalseStatus_ReturnsListWithFalseStatus()
        {
            // Arrange
            var prescriptions = new List<Prescription>
    {
        new Prescription { PrescriptionId = 3, IssueDate = DateTime.Now, Status = false, IsBhyt = true }
    };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(3, result[0].PrescriptionId);
            Assert.False(result[0].Status);
        }

        [Fact]
        public async Task GetAllPrescriptionsAsync_MixedPrescriptions_ReturnsOnlyBhyt()
        {
            // Arrange
            var prescriptions = new List<Prescription>
    {
        new Prescription { PrescriptionId = 1, IssueDate = DateTime.Now, Status = true, IsBhyt = true },
        new Prescription { PrescriptionId = 2, IssueDate = DateTime.Now, Status = false, IsBhyt = true },
        new Prescription { PrescriptionId = 3, IssueDate = DateTime.Now, Status = true, IsBhyt = false },
    };
            _repositoryMock.Setup(r => r.GetAllPrescriptionsAsync())
                           .ReturnsAsync(prescriptions);

            // Act
            var result = await _service.GetAllPrescriptionsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.True(p.IsBhyt));
        }
    }
}