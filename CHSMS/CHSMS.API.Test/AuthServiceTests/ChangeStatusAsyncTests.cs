using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests
{
    public class ChangeStatusAsyncTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public ChangeStatusAsyncTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();

            // Setup configuration values (using your TestHelper's key format)
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("This Is A Super Long Secret Key With More Than Enough Length For HS512");
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
        public async Task ChangeStatusAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepositoryMock.Setup(u => u.GetByIdAsync(-1))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.ChangeStatusAsync(-1));
        }

        [Fact]
        public async Task ChangeStatusAsync_ValidUser_TogglesStatus()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.Status = true;
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(u => u.CountActiveUser())
                .ReturnsAsync(0); // Below activation limit

            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.ChangeStatusAsync(1);

            // Assert
            Assert.True(result);
            Assert.False(user.Status);
            _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async Task ChangeStatusAsync_TooManyActiveUsers_ThrowsException()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.Status = false;
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(u => u.CountActiveUser())
                .ReturnsAsync(10); // At activation limit

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.ChangeStatusAsync(1));
        }
    }
}