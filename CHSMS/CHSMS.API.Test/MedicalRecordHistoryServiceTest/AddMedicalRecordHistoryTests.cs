using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class AddMedicalRecordHistoryTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public AddMedicalRecordHistoryTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void AddMedicalRecordHistory_ValidInput_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            var dto = TestHelper.CreateDefaultMedicalRecordHistoryDTO();

            _repositoryMock.Setup(r => r.AddMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()))
                .Returns(true);

            // Act
            var result = _service.AddMedicalRecordHistory(userId, dto);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.AddMedicalRecordHistory(It.Is<MedicalRecordHistory>(m =>
                m.MedicalRecordId == dto.PatientId &&
                m.UserId == userId &&
                m.DiagnoseConclusion == dto.DiagnoseConclusion &&
                m.TreatmentMethod == dto.TreatmentMethod &&
                m.Symptom == dto.Symptom
            )), Times.Once());
        }

        [Fact]
        public void AddMedicalRecordHistory_RepositoryFailure_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            var dto = TestHelper.CreateDefaultMedicalRecordHistoryDTO();

            _repositoryMock.Setup(r => r.AddMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()))
                .Returns(false);

            // Act
            var result = _service.AddMedicalRecordHistory(userId, dto);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.AddMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()), Times.Once());
        }


    }
}
