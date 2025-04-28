/*using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class DeleteMedicalRecordHistoryTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public DeleteMedicalRecordHistoryTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void DeleteMedicalRecordHistory_ValidId_ReturnsTrue()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteMedicalRecordHistory(1))
                .Returns(true);

            // Act
            var result = _service.DeleteMedicalRecordHistory(1);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.DeleteMedicalRecordHistory(1), Times.Once());
        }

        [Fact]
        public void DeleteMedicalRecordHistory_RepositoryFailure_ReturnsFalse()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteMedicalRecordHistory(1))
                .Returns(false);

            // Act
            var result = _service.DeleteMedicalRecordHistory(0);

            // Assert
            Assert.False(result);
        }
    }
}
*/