using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetMedicalRecordHistoryByPatientIdTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetMedicalRecordHistoryByPatientIdTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_ReturnsCorrectDTOs()
        {
            // Arrange
            var mockRecords = TestHelper.CreateDefaultMedicalRecordHistories()
                .Where(r => r.MedicalRecordId == 101).ToList();

            _repositoryMock.Setup(r => r.GetMedicalRecordHistoryByPatientId(101, null, null, null))
                .Returns(mockRecords);

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(101, null, null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MedicalRecordHistoryId);
            Assert.Equal("John Doe", result[0].PatientName);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_WithDateFilter_ReturnsFilteredDTOs()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-2);
            var endDate = DateTime.Now;
            var mockRecords = TestHelper.CreateDefaultMedicalRecordHistories()
                .Where(r => r.MedicalRecordId == 101 && r.Date >= startDate && r.Date <= endDate).ToList();

            _repositoryMock.Setup(r => r.GetMedicalRecordHistoryByPatientId(101, startDate, endDate, null))
                .Returns(mockRecords);

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(101, startDate, endDate, null);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MedicalRecordHistoryId);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_WithDoctorNameFilter_ReturnsFilteredDTOs()
        {
            // Arrange
            string doctorName = "Dr. Smith";
            var mockRecords = TestHelper.CreateDefaultMedicalRecordHistories()
                .Where(r => r.MedicalRecordId == 101 && r.User.UserName == doctorName).ToList();

            _repositoryMock.Setup(r => r.GetMedicalRecordHistoryByPatientId(101, null, null, doctorName))
                .Returns(mockRecords);

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(101, null, null, doctorName);

            // Assert
            Assert.Single(result);
            Assert.Equal("Dr. Smith", result[0].DoctorName);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_IdNotExist_ReturnsEmptyList()
        {
            // Arrange
            int nonExistentId = 999;
            _repositoryMock.Setup(r => r.GetMedicalRecordHistoryByPatientId(nonExistentId, null, null, null))
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(nonExistentId, null, null, null);

            // Assert
            Assert.Empty(result);
            Assert.IsType<List<MedicalRecordHistoryDTO>>(result);
        }

        [Fact]
        public void GetMedicalRecordHistoryByPatientId_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetMedicalRecordHistoryByPatientId(It.IsAny<int>(), null, null, null))
                .Returns(new List<MedicalRecordHistory>());

            // Act
            var result = _service.GetMedicalRecordHistoryByPatientId(101, null, null, null);

            // Assert
            Assert.Empty(result);
        }
    }
}
