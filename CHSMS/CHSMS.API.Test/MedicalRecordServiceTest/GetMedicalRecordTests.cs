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

        [Fact]
        public void GetMedicalRecord_WithNullFields_ReturnsDTOWithNullFields()
        {
            // Arrange
            var testRecord = new MedicalRecord
            {
                MedicalRecordId = 2,
                PatientName = null,
                Gender = null,
                Dob = null,
                EthnicGroup = null,
                EducationLevel = null,
                HealthInsurance = null,
                Address = null,
                PhoneNumber = null,
                Email = null,
                Job = null,
                Status = null,
                Note = null
            };

            _mockRepo.Setup(x => x.GetMedicalRecord(2)).Returns(testRecord);

            // Act
            var result = _service.GetMedicalRecord(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.MedicalRecordId);
            Assert.Null(result.PatientName);
            Assert.Null(result.Gender);
            Assert.Null(result.Dob);
            Assert.Null(result.EthnicGroup);
            Assert.Null(result.EducationLevel);
            Assert.Null(result.HealthInsurance);
            Assert.Null(result.Address);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.Email);
            Assert.Null(result.Job);
            Assert.Null(result.Status);
            Assert.Null(result.Note);
        }

        [Fact]
        public void GetMedicalRecord_WithPartialNullFields_ReturnsCorrectDTO()
        {
            // Arrange
            var testRecord = new MedicalRecord
            {
                MedicalRecordId = 3,
                PatientName = "Trần Thị B",
                Gender = "Nữ",
                Dob = new DateTime(1985, 5, 15),
                EthnicGroup = null,
                EducationLevel = "Cao đẳng",
                HealthInsurance = null,
                Address = "TP HCM",
                PhoneNumber = "0912345678",
                Email = null,
                Job = "Giáo viên",
                Status = false,
                Note = null
            };

            _mockRepo.Setup(x => x.GetMedicalRecord(3)).Returns(testRecord);

            // Act
            var result = _service.GetMedicalRecord(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.MedicalRecordId);
            Assert.Equal("Trần Thị B", result.PatientName);
            Assert.Equal("Nữ", result.Gender);
            Assert.Equal(new DateTime(1985, 5, 15), result.Dob);
            Assert.Null(result.EthnicGroup);
            Assert.Equal("Cao đẳng", result.EducationLevel);
            Assert.Null(result.HealthInsurance);
            Assert.Equal("TP HCM", result.Address);
            Assert.Equal("0912345678", result.PhoneNumber);
            Assert.Null(result.Email);
            Assert.Equal("Giáo viên", result.Job);
            Assert.Equal(false, result.Status);
            Assert.Null(result.Note);
        }
    }
}
