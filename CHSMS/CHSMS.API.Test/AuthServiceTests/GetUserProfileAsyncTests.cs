using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Tests.AuthServiceTests
{
    public class GetUserProfileAsyncTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public GetUserProfileAsyncTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();

            // Only need to mock dependencies that are actually used by GetUserProfileAsync
            _authService = new AuthService(
                _userRepositoryMock.Object,
                Mock.Of<IRoleRepository>(),  // Not used in this method
                Mock.Of<IConfiguration>(),   // Not used in this method
                Mock.Of<IEmailService>(),    // Not used in this method
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetUserProfileAsync_ValidId_ReturnsMappedUser()
        {
            // Arrange
            var userId = 1;
            var user = TestHelper.CreateTestUser(userId);
            var expectedDto = new UserListDto
            {
                UserId = userId,
                Username = user.UserName,
                Fullname = user.Fullname,
                Email = user.Email
                // Include other relevant properties
            };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserListDto>(user))
                .Returns(expectedDto);

            // Act
            var result = await _authService.GetUserProfileAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.UserId, result.UserId);
            Assert.Equal(expectedDto.Username, result.Username);
            // Add more property assertions as needed

            _userRepositoryMock.Verify(u => u.GetByIdAsync(userId), Times.Once);
            _mapperMock.Verify(m => m.Map<UserListDto>(user), Times.Once);
        }

        [Fact]
        public async Task GetUserProfileAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = -1;
            _userRepositoryMock.Setup(u => u.GetByIdAsync(invalidId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.GetUserProfileAsync(invalidId);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(u => u.GetByIdAsync(invalidId), Times.Once);

            // Verify mapper wasn't called when user is null
            _mapperMock.Verify(m => m.Map<UserListDto>(It.IsAny<User>()), Times.Never);
        }
    }
}