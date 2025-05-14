using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class AddMedicalRecordHistoryTests
    {
        protected readonly Mock<IMedicalRecordRepository> _mockRepository;
        protected readonly MedicalRecordService _service;

        public AddMedicalRecordHistoryTests()
        {
            _mockRepository = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepository.Object);
        }

        [Fact]
        public void AddMedicalRecordHistory_WhenRepositoryFails_ShouldReturnFalse()
        {
            // Arrange
            var recordDTO = TestHelper.CreateTestMedicalRecordDTO();

            _mockRepository.Setup(repo => repo.AddMedicalRecordHistory(It.IsAny<MedicalRecord>()))
                .Returns(false);

            // Act
            var result = _service.AddMedicalRecord(recordDTO);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.AddMedicalRecordHistory(It.IsAny<MedicalRecord>()), Times.Once);
        }

        public void AddMedicalRecordHistory_WithValidRecord_ShouldReturnTrue()
        {
            // Arrange
            var recordDTO = TestHelper.CreateTestMedicalRecordDTO();
            MedicalRecord capturedRecord = null;

            _mockRepository.Setup(repo => repo.AddMedicalRecordHistory(It.IsAny<MedicalRecord>()))
                .Callback<MedicalRecord>(record => capturedRecord = record)
                .Returns(true);

            // Act
            var result = _service.AddMedicalRecord(recordDTO);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddMedicalRecordHistory(It.IsAny<MedicalRecord>()), Times.Once);
            Assert.NotNull(capturedRecord);
            TestHelper.VerifyMedicalRecordMatchesDTO(capturedRecord, recordDTO); // Verify mapping
        }
    }
}
