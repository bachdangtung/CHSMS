using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class GetAllMedicalRecordsByPatientNameTests
    {
        protected readonly Mock<IMedicalRecordRepository> _mockRepository;
        protected readonly MedicalRecordService _service;

        public GetAllMedicalRecordsByPatientNameTests()
        {
            _mockRepository = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepository.Object);
        }
        [Fact]
        public void GetAllMedicalRecordsByPatientName_WithValidName_ShouldReturnMatchingRecords()
        {
            // Arrange
            var testRecords = TestHelper.CreateTestMedicalRecords();
            var patientName = "John";
            var filteredRecords = testRecords.FindAll(r => r.PatientName.Contains(patientName));

            _mockRepository.Setup(repo => repo.GetMedicalRecordsByPatientName(patientName)).Returns(filteredRecords);

            // Act
            var result = _service.GetAllMedicalRecordsByPatientName(patientName);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            TestHelper.VerifyMedicalRecordMatchesDTO(filteredRecords[0], result[0]);
            _mockRepository.Verify(repo => repo.GetMedicalRecordsByPatientName(patientName), Times.Once);
        }

        [Fact]
        public void GetAllMedicalRecordsByPatientName_WithNonExistentName_ShouldReturnEmptyList()
        {
            // Arrange
            var patientName = "NonExistent";
            _mockRepository.Setup(repo => repo.GetMedicalRecordsByPatientName(patientName))
                .Returns(new List<MedicalRecord>());

            // Act
            var result = _service.GetAllMedicalRecordsByPatientName(patientName);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetMedicalRecordsByPatientName(patientName), Times.Once);
        }

        [Fact]
        public void GetAllMedicalRecordsByPatientName_WithNullName_ShouldHandleNull()
        {
            // Arrange
            string patientName = null;
            var testRecords = TestHelper.CreateTestMedicalRecords();

            _mockRepository.Setup(repo => repo.GetMedicalRecordsByPatientName(patientName))
                .Returns(testRecords);

            // Act
            var result = _service.GetAllMedicalRecordsByPatientName(patientName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testRecords.Count, result.Count);
            _mockRepository.Verify(repo => repo.GetMedicalRecordsByPatientName(patientName), Times.Once);
        }
    }

}
