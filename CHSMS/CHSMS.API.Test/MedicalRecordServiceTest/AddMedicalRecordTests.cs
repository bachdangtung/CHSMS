using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class AddMedicalRecordTests
    {
        private readonly Mock<IMedicalRecordRepository> _mockRepo;
        private readonly MedicalRecordService _service;

        public AddMedicalRecordTests()
        {
            _mockRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepo.Object);

            // Setup precondition: Existing record with PhoneNumber, Email, and HealthInsurance
            var existingRecords = new List<MedicalRecord>
            {
                new MedicalRecord
                {
                    PhoneNumber = "0123456789",
                    Email = "test@example.com",
                    HealthInsurance = "DN2790987654321"
                }
            };
            _mockRepo.Setup(repo => repo.GetAllMedicalRecords()).Returns(existingRecords);
            _mockRepo.Setup(repo => repo.AddMedicalRecordHistory(It.IsAny<MedicalRecord>())).Returns(true);
        }

        [Fact]
        public void AddMedicalRecord_ValidInputs_Success()
        {
            // Arrange
            var validDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Gender = "Nam",
                Address = "Hà Nội",
                EthnicGroup = "Kinh",
                EducationLevel = "Đại học",
                Job = "Doanh nhân",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0987654321",
                Email = "new@example.com"
            };

            // Act
            var result = _service.AddMedicalRecord(validDto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AddMedicalRecord_FutureDob_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(2026, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0987654321",
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Ngày sinh không thể lớn hơn ngày hiện tại!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_TooOldDob_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1800, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0987654321",
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_InvalidHealthInsuranceFormat_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "ABCD1234",
                PhoneNumber = "0987654321",
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_DuplicateHealthInsurance_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790987654321", // Duplicate with precondition
                PhoneNumber = "0987654321",
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Số bảo hiểm y tế đã tồn tại!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_InvalidPhoneNumberFormat_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "abcd1234",
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Số điện thoại phải có 10-11 chữ số!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_DuplicatePhoneNumber_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0123456789", // Duplicate with precondition
                Email = "new@example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Số điện thoại đã tồn tại!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_InvalidEmailFormat_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0987654321",
                Email = "abcd1234"
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Email không hợp lệ (vd: email@domain.com)!", ex.Message);
        }

        [Fact]
        public void AddMedicalRecord_DuplicateEmail_ThrowsException()
        {
            // Arrange
            var invalidDto = new MedicalRecordDTO
            {
                PatientName = "Phạm Dương Thanh Quý",
                Dob = new DateTime(1995, 1, 1),
                HealthInsurance = "DN2790123456789",
                PhoneNumber = "0987654321",
                Email = "test@example.com" // Duplicate with precondition
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecord(invalidDto));
            Assert.Equal("Email đã tồn tại!", ex.Message);
        }
    }
}
