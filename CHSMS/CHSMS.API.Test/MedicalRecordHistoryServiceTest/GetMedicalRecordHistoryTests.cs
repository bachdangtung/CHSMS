using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;
using static CHSMS.API.Test.MedicalRecordHistoryServiceTest.GetAllMedicalRecordHistoriesTests;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetMedicalRecordHistoryTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetMedicalRecordHistoryTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetMedicalRecordHistory_ExistingRecord_ReturnsCorrectDTO()
        {
            // Arrange
            var mockRecord = TestHelper.CreateDefaultMedicalRecordHistories()[0];
            _repositoryMock.Setup(r => r.GetMedicalRecordHistory(1))
                .Returns(mockRecord);

            // Act
            var result = _service.GetMedicalRecordHistory(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicalRecordHistoryId);
            Assert.Equal(101, result.PatientId);
            Assert.Equal("John Doe", result.PatientName);
            Assert.Equal("Dr. Smith", result.DoctorName);
            Assert.Equal("High fever", result.Symptom);
        }

        [Fact]
        public void GetMedicalRecordHistory_NonExistingRecord_ReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetMedicalRecordHistory(99))
                .Returns((MedicalRecordHistory)null);

            // Act
            var result = _service.GetMedicalRecordHistory(99);

            // Assert
            Assert.Null(result);
        }
    }
}
