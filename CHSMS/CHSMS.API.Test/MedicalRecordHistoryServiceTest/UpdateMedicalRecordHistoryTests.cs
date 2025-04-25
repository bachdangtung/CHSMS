using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class UpdateMedicalRecordHistoryTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public UpdateMedicalRecordHistoryTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void UpdateMedicalRecordHistory_ExistingRecord_ReturnsTrue()
        {
            // Arrange
            var dto = TestHelper.CreateDefaultMedicalRecordHistoryDTO();
            dto.MedicalRecordHistoryId = 1;
            dto.DiagnoseConclusion = "Updated diagnosis";

            var existingRecord = TestHelper.CreateDefaultMedicalRecordHistories()[0];

            _repositoryMock.Setup(r => r.GetMedicalRecordHistory(1))
                .Returns(existingRecord);
            _repositoryMock.Setup(r => r.UpdateMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()))
                .Returns(true);

            // Act
            var result = _service.UpdateMedicalRecordHistory(dto);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.UpdateMedicalRecordHistory(It.Is<MedicalRecordHistory>(m =>
                m.MedicalRecordHistoryId == dto.MedicalRecordHistoryId &&
                m.DiagnoseConclusion == dto.DiagnoseConclusion
            )), Times.Once());
        }

        [Fact]
        public void UpdateMedicalRecordHistory_NonExistingRecord_ReturnsFalse()
        {
            // Arrange
            var dto = TestHelper.CreateDefaultMedicalRecordHistoryDTO();
            dto.MedicalRecordHistoryId = 99;

            _repositoryMock.Setup(r => r.GetMedicalRecordHistory(99))
                .Returns((MedicalRecordHistory)null);

            // Act
            var result = _service.UpdateMedicalRecordHistory(dto);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.UpdateMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()), Times.Never());
        }

        [Fact]
        public void UpdateMedicalRecordHistory_RepositoryFailure_ReturnsFalse()
        {
            // Arrange
            var dto = TestHelper.CreateDefaultMedicalRecordHistoryDTO();
            dto.MedicalRecordHistoryId = 1;
            var existingRecord = TestHelper.CreateDefaultMedicalRecordHistories()[0];

            _repositoryMock.Setup(r => r.GetMedicalRecordHistory(1))
                .Returns(existingRecord);
            _repositoryMock.Setup(r => r.UpdateMedicalRecordHistory(It.IsAny<MedicalRecordHistory>()))
                .Returns(false);

            // Act
            var result = _service.UpdateMedicalRecordHistory(dto);

            // Assert
            Assert.False(result);
        }
    }
}
