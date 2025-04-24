using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetMedicalRecordHistoriesByFilterTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetMedicalRecordHistoriesByFilterTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_ReturnsFilteredDTOs()
        {
            // Arrange
            var mockRecords = TestHelper.CreateDefaultMedicalRecordHistories()
                .Where(r => r.User.UserName == "Dr. Smith" && r.MedicalRecord.PatientName.Contains("John")).ToList();

            _repositoryMock.Setup(r => r.GetMedicalRecordHistoriesByFilter("Dr. Smith", "John"))
                .Returns(mockRecords);

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("Dr. Smith", "John");

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MedicalRecordHistoryId);
            Assert.Equal("John Doe", result[0].PatientName);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_NoMatches_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetMedicalRecordHistoriesByFilter("Dr. Unknown", "Unknown"))
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("Dr. Unknown", "Unknown");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetMedicalRecordHistoriesByFilter_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetMedicalRecordHistoriesByFilter(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetMedicalRecordHistoriesByFilter("Dr. Smith", "John");

            // Assert
            Assert.Empty(result);
        }
    }
}
