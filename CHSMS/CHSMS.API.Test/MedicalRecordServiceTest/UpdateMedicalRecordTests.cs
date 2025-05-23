using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class UpdateMedicalRecordTests
    {
        private readonly MedicalRecordService _service;
        private readonly Mock<IMedicalRecordRepository> _mockRepo;

        public UpdateMedicalRecordTests()
        {
            _mockRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepo.Object);

            // Setup initial test data
            var existingRecord1 = new MedicalRecord { MedicalRecordId = 1 };
            var existingRecord2 = new MedicalRecord
            {
                MedicalRecordId = 2,
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                HealthInsurance = "DN2790987654321"
            };

            _mockRepo.Setup(x => x.GetMedicalRecord(1)).Returns(existingRecord1);
            _mockRepo.Setup(x => x.GetMedicalRecord(2)).Returns(existingRecord2);

            var allRecords = new List<MedicalRecord> { existingRecord1, existingRecord2 };
            _mockRepo.Setup(x => x.GetAllMedicalRecords()).Returns(allRecords);
        }

        [Fact]
        public void UpdateMedicalRecord_WithInvalidId_ThrowsException()
        {
            var dto = new MedicalRecordDTO { MedicalRecordId = -1 };
            Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
        }

        [Fact]
        public void UpdateMedicalRecord_WithValidData_UpdatesSuccessfully()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
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

            _mockRepo.Setup(x => x.UpdateMedicalRecord(It.IsAny<MedicalRecord>())).Returns(true);

            var result = _service.UpdateMedicalRecord(dto);
            Assert.True(result);
        }

        [Theory]
        [InlineData("1/1/2026", "Ngày sinh không thể lớn hơn ngày hiện tại!")]
        [InlineData("1/1/1800", "Tuổi bệnh nhân không hợp lệ (tối đa 150 tuổi)!")]
        public void UpdateMedicalRecord_WithInvalidDob_ThrowsException(string dob, string expectedError)
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                Dob = DateTime.Parse(dob)
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithInvalidPhoneNumber_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                PhoneNumber = "abcd1234"
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Số điện thoại phải có 10-11 chữ số!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithDuplicatePhoneNumber_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                PhoneNumber = "0123456789" // Duplicate with record 2
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Số điện thoại đã tồn tại!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithInvalidEmail_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                Email = "abcd1234"
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Email không hợp lệ (vd: email@domain.com)!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithDuplicateEmail_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                Email = "test@example.com" // Duplicate with record 2
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Email đã tồn tại!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithInvalidHealthInsurance_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                HealthInsurance = "ABCD1234"
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Số bảo hiểm y tế phải có 15 ký tự (2 chữ cái đầu + 13 số)!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithDuplicateHealthInsurance_ThrowsException()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 1,
                HealthInsurance = "DN2790987654321" // Duplicate with record 2
            };

            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecord(dto));
            Assert.Equal("Số bảo hiểm y tế đã tồn tại!", ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecord_WithSamePhoneNumberForSameRecord_DoesNotThrow()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 2, // Same record
                PhoneNumber = "0123456789" // Same phone number
            };

            _mockRepo.Setup(x => x.UpdateMedicalRecord(It.IsAny<MedicalRecord>())).Returns(true);

            var result = _service.UpdateMedicalRecord(dto);
            Assert.True(result);
        }

        [Fact]
        public void UpdateMedicalRecord_WithSameEmailForSameRecord_DoesNotThrow()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 2, // Same record
                Email = "test@example.com" // Same email
            };

            _mockRepo.Setup(x => x.UpdateMedicalRecord(It.IsAny<MedicalRecord>())).Returns(true);

            var result = _service.UpdateMedicalRecord(dto);
            Assert.True(result);
        }

        [Fact]
        public void UpdateMedicalRecord_WithSameHealthInsuranceForSameRecord_DoesNotThrow()
        {
            var dto = new MedicalRecordDTO
            {
                MedicalRecordId = 2, // Same record
                HealthInsurance = "DN2790987654321" // Same health insurance
            };

            _mockRepo.Setup(x => x.UpdateMedicalRecord(It.IsAny<MedicalRecord>())).Returns(true);

            var result = _service.UpdateMedicalRecord(dto);
            Assert.True(result);
        }
    }
}
