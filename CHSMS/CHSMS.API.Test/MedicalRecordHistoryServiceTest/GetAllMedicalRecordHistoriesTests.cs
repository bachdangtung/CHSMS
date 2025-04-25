using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetAllMedicalRecordHistoriesTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetAllMedicalRecordHistoriesTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetAllMedicalRecordHistories_ReturnsCorrectDTOs()
        {
            // Arrange
            var mockRecords = TestHelper.CreateDefaultMedicalRecordHistories();
            _repositoryMock.Setup(r => r.GetAllMedicalRecordHistories())
                .Returns(mockRecords);

            // Act
            var result = _service.GetAllMedicalRecordHistories();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicalRecordHistoryId);
            Assert.Equal("John Doe", result[0].PatientName);
            Assert.Equal("Dr. Smith", result[0].DoctorName);
            Assert.Equal(2, result[1].MedicalRecordHistoryId);
            Assert.Equal("Jane Smith", result[1].PatientName);
        }

        [Fact]
        public void GetAllMedicalRecordHistories_EmptyList_ReturnsEmptyDTOs()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllMedicalRecordHistories())
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetAllMedicalRecordHistories();

            // Assert
            Assert.Empty(result);
        }

    }
}


