using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class GetMedicalRecordTests
    {
        private readonly MedicalRecordService _service;
        private readonly Mock<IMedicalRecordRepository> _mockRepo;

        public GetMedicalRecordTests()
        {
            _mockRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepo.Object);
        }

        [Fact]
        public void GetMedicalRecord_WithExistingId_ReturnsCorrectRecord()
        {
            // Arrange
            var testRecord = new MedicalRecord
            {
                MedicalRecordId = 1,
                PatientName = "Nguyễn Văn A",
                Gender = "Nam",
                Dob = new DateTime(1990, 1, 1),
                EthnicGroup = "Kinh",
                EducationLevel = "Đại học",
                HealthInsurance = "DN1234567890123",
                Address = "Hà Nội",
                PhoneNumber = "0987654321",
                Email = "test@example.com",
                Job = "Kỹ sư",
                Status = true,
                Note = "Test note"
            };

            _mockRepo.Setup(x => x.GetMedicalRecord(1)).Returns(testRecord);

            // Act
            var result = _service.GetMedicalRecord(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MedicalRecordId);
            Assert.Equal("Nguyễn Văn A", result.PatientName);
            Assert.Equal("Nam", result.Gender);
            Assert.Equal(new DateTime(1990, 1, 1), result.Dob);
            Assert.Equal("Kinh", result.EthnicGroup);
            Assert.Equal("Đại học", result.EducationLevel);
            Assert.Equal("DN1234567890123", result.HealthInsurance);
            Assert.Equal("Hà Nội", result.Address);
            Assert.Equal("0987654321", result.PhoneNumber);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Kỹ sư", result.Job);
            Assert.Equal(true, result.Status);
            Assert.Equal("Test note", result.Note);
        }

        [Fact]
        public void GetMedicalRecord_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetMedicalRecord(999)).Returns((MedicalRecord)null);

            // Act
            var result = _service.GetMedicalRecord(999);

            // Assert
            Assert.Null(result);
        }
    }
}
