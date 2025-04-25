using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetTodayMedicalRecordHistoryCountTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetTodayMedicalRecordHistoryCountTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetTodayMedicalRecordHistoryCount_ReturnsCorrectCount()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CountTodayMedicalRecordHistories())
                .Returns(5);

            // Act
            var result = _service.GetTodayMedicalRecordHistoryCount();

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void GetTodayMedicalRecordHistoryCount_NoRecords_ReturnsZero()
        {
            // Arrange
            _repositoryMock.Setup(r => r.CountTodayMedicalRecordHistories())
                .Returns(0);

            // Act
            var result = _service.GetTodayMedicalRecordHistoryCount();

            // Assert
            Assert.Equal(0, result);
        }
    }
}
