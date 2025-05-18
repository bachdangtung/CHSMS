using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using Moq;

namespace CHSMS.API.Tests.Services
{
    public class MedicalRecordHistoryService_AddTests : MedicalRecordHistoryServiceTestBase
    {
        [Fact]
        public void AddMedicalRecordHistory_ValidInput_ReturnsTrue()
        {
            // Arrange
            var dto = new MedicalRecordHistoryDTO
            {
                PatientId = 1,
                MedicalRecordHistoryCode = "CHSMSMRH001",
                PatientCategory = "Bệnh nhân",
                DiagnoseConclusion = "Có bệnh",
                TreatmentMethod = "Uống thuốc",
                DiseaseProgress = "Đã khỏi",
                MedicalOrder = "CHSMSMO001",
                Symptom = "Ho, nôn mửa",
                Pulse = 150,
                RespiratoryRate = 30,
                Temperature = 37,
                Height = 180,
                Weight = 80,
                BloodPressure = "120/80",
                RecordDate = DateTime.Now
            };

            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(1)).Returns(new MedicalRecord());
            _mockHistoryRepo.Setup(x => x.AddMedicalRecordHistory(It.IsAny<MedicalRecordHistory>())).Returns(true);

            // Act
            var result = _service.AddMedicalRecordHistory(1, dto);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(-1, "ID bệnh án không được để trống!")]
        public void AddMedicalRecordHistory_InvalidMedicalRecordId_ThrowsException(int patientId, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.PatientId = patientId;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(patientId)).Returns((MedicalRecord)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(20, "Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!")]
        [InlineData(300, "Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!")]
        public void AddMedicalRecordHistory_InvalidPulse_ThrowsException(int pulse, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.Pulse = pulse;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(0, "Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!")]
        [InlineData(100, "Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!")]
        public void AddMedicalRecordHistory_InvalidRespiratoryRate_ThrowsException(int rate, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.RespiratoryRate = rate;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(30, "Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!")]
        [InlineData(50, "Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!")]
        public void AddMedicalRecordHistory_InvalidTemperature_ThrowsException(int temp, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.Temperature = temp;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(20, "Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!")]
        [InlineData(300, "Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!")]
        public void AddMedicalRecordHistory_InvalidHeight_ThrowsException(int height, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.Height = height;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(0, "Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!")]
        [InlineData(500, "Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!")]
        public void AddMedicalRecordHistory_InvalidWeight_ThrowsException(int weight, string expectedError)
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.Weight = weight;
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Fact]
        public void AddMedicalRecordHistory_InvalidBloodPressureFormat_ThrowsException()
        {
            // Arrange
            var dto = CreateValidMedicalRecordHistoryDTO();
            dto.BloodPressure = "120";
            _mockMedicalRecordRepo.Setup(x => x.GetMedicalRecord(dto.PatientId)).Returns(new MedicalRecord());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.AddMedicalRecordHistory(1, dto));
            Assert.Equal("Huyết áp phải có định dạng 'số/số' (ví dụ: 120/80)!", ex.Message);
        }
    }
}