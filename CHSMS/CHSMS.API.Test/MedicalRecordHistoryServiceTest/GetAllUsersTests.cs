using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalRecordHistoryServiceTest
{
    public class GetAllUsersTests
    {
        private readonly Mock<IMedicalRecordHistoryRepository> _repositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly MedicalRecordHistoryService _service;

        public GetAllUsersTests()
        {
            _repositoryMock = new Mock<IMedicalRecordHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new MedicalRecordHistoryService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetAllUsers_ReturnsCorrectDTOs()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User { UserId = 1, UserName = "Dr. Smith", Gender = "Male" },
                new User { UserId = 2, UserName = "Dr. Johnson", Gender = "Female" }
            };

            _repositoryMock.Setup(r => r.GetAllUsers())
                .Returns(mockUsers);

            // Act
            var result = _service.GetAllUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].UserId);
            Assert.Equal("Dr. Smith", result[0].UserName);
            Assert.Equal("Male", result[0].Gender);
        }

        [Fact]
        public void GetAllUsers_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllUsers())
                .Returns(new List<User>());

            // Act
            var result = _service.GetAllUsers();

            // Assert
            Assert.Empty(result);
        }
    }
}
