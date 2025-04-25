using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordServiceTest
{
    public class GetAllMedicalRecordsTests
    {
        protected readonly Mock<IMedicalRecordRepository> _mockRepository;
        protected readonly MedicalRecordService _service;

        public GetAllMedicalRecordsTests()
        {
            _mockRepository = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_mockRepository.Object);
        }

        [Fact]
        public void GetAllMedicalRecords_ShouldReturnAllRecords()
        {
            // Arrange
            var testRecords = TestHelper.CreateTestMedicalRecords();
            _mockRepository.Setup(repo => repo.GetAllMedicalRecords()).Returns(testRecords);

            // Act
            var result = _service.GetAllMedicalRecords();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testRecords.Count, result.Count);

            for (int i = 0; i < testRecords.Count; i++)
            {
                TestHelper.VerifyMedicalRecordMatchesDTO(testRecords[i], result[i]);
            }

            _mockRepository.Verify(repo => repo.GetAllMedicalRecords(), Times.Once);
        }

        [Fact]
        public void GetAllMedicalRecords_WhenNoRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllMedicalRecords()).Returns(new List<MedicalRecord>());

            // Act
            var result = _service.GetAllMedicalRecords();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetAllMedicalRecords(), Times.Once);
        }
    }
}
