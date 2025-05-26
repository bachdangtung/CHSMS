using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Models;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class MedicalRecordHistoryService_UpdateTests : MedicalRecordHistoryServiceTestBase
    {
        private MedicalRecordHistoryDTO CreateValidUpdateDTO()
        {
            return new MedicalRecordHistoryDTO
            {
                MedicalRecordHistoryId = 1,
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
        }

        [Fact]
        public void UpdateMedicalRecordHistory_ValidInput_ReturnsTrue()
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            var existingRecord = new MedicalRecordHistory { MedicalRecordHistoryId = 1 };

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(existingRecord);
            _mockHistoryRepo.Setup(x => x.UpdateMedicalRecordHistory(It.IsAny<MedicalRecordHistory>())).Returns(true);

            // Act
            var result = _service.UpdateMedicalRecordHistory(dto);

            // Assert
            Assert.True(result);
            _mockHistoryRepo.Verify(x => x.UpdateMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()), Times.Once);
        }

        [Fact]
        public void UpdateMedicalRecordHistory_InvalidId_ThrowsException()
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.MedicalRecordHistoryId = -1;

            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(-1)).Returns((MedicalRecordHistory)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal("Lịch sử bệnh án không tồn tại!", ex.Message);
        }

        [Theory]
        [InlineData(20, "Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!")]
        [InlineData(300, "Mạch phải nằm trong khoảng 30 bpm đến 200 bpm!")]
        public void UpdateMedicalRecordHistory_InvalidPulse_ThrowsException(int pulse, string expectedError)
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.Pulse = pulse;
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(0, "Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!")]
        [InlineData(100, "Nhịp thở phải nằm trong khoảng 10 lần/phút đến 60 lần/phút!")]
        public void UpdateMedicalRecordHistory_InvalidRespiratoryRate_ThrowsException(int rate, string expectedError)
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.RespiratoryRate = rate;
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(30, "Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!")]
        [InlineData(50, "Nhiệt độ phải nằm trong khoảng 33°C đến 45°C!")]
        public void UpdateMedicalRecordHistory_InvalidTemperature_ThrowsException(int temp, string expectedError)
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.Temperature = temp;
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(20, "Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!")]
        [InlineData(300, "Chiều cao phải nằm trong khoảng 30 cm đến 250 cm!")]
        public void UpdateMedicalRecordHistory_InvalidHeight_ThrowsException(int height, string expectedError)
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.Height = height;
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Theory]
        [InlineData(0, "Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!")]
        [InlineData(500, "Cân nặng phải nằm trong khoảng 1 kg đến 300 kg!")]
        public void UpdateMedicalRecordHistory_InvalidWeight_ThrowsException(int weight, string expectedError)
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.Weight = weight;
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal(expectedError, ex.Message);
        }

        [Fact]
        public void UpdateMedicalRecordHistory_InvalidBloodPressureFormat_ThrowsException()
        {
            // Arrange
            var dto = CreateValidUpdateDTO();
            dto.BloodPressure = "120";
            _mockHistoryRepo.Setup(x => x.GetMedicalRecordHistory(1)).Returns(new MedicalRecordHistory());

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _service.UpdateMedicalRecordHistory(dto));
            Assert.Equal("Huyết áp phải có định dạng 'số/số' (ví dụ: 120/80)!", ex.Message);
        }
    }
}