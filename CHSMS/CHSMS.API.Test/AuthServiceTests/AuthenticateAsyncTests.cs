using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests
{
    public class AuthenticateAsyncTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public AuthenticateAsyncTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();

            // Setup configuration values
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASuperLongSecretKeyWithMoreThanEnoughLengthForHS512");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("30");
            _configurationMock.Setup(c => c["Jwt:RefreshTokenExpiryInDays"]).Returns("7");

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _configurationMock.Object,
                _emailServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("nonexistent"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync("nonexistent", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_IncorrectPassword_ReturnsNull()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsInactiveToken()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.Status = false;
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("inactive", result.AccessToken);
            Assert.Null(result.RefreshToken);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTokenPair()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);

            // Mock UpdateAsync (which now handles SaveChanges)
            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiry > DateTime.UtcNow);

            // Verify UpdateAsync was called (no need to check SaveChanges)
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
        }
    }
}