using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class GetAllMedicalRecordsTests
    {
        private readonly Mock<IMedicalRecordRepository> _mockRepo;
        private readonly MedicalRecordService _service;

        public GetAllMedicalRecordsTests()
        {
            _mockRepo = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepo.Object);
        }

        [Fact]
        public void GetAllMedicalRecords_ReturnsAllRecords()
        {
            // Arrange
            var records = new List<MedicalRecord>
            {
                new MedicalRecord { MedicalRecordId = 1, PatientName = "Nguyễn Văn A" },
                new MedicalRecord { MedicalRecordId = 2, PatientName = "Nguyễn Văn B" }
            };
            _mockRepo.Setup(repo => repo.GetAllMedicalRecords()).Returns(records);

            // Act
            var result = _service.GetAllMedicalRecords();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].PatientName);
            Assert.Equal("Jane Smith", result[1].PatientName);
        }

        [Fact]
        public void GetAllMedicalRecords_ReturnsEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllMedicalRecords()).Returns(new List<MedicalRecord>());

            // Act
            var result = _service.GetAllMedicalRecords();

            // Assert
            Assert.Empty(result);
        }
    }
}
