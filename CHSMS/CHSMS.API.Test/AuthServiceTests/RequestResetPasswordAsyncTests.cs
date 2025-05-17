using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests;

public class RequestResetPasswordAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AuthService _authService;
    public RequestResetPasswordAsyncTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _emailServiceMock = new Mock<IEmailService>();
        _mapperMock = new Mock<IMapper>();

        // Setup configuration values
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
    public async Task RequestResetPasswordAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("nonexistent@example.com"))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.RequestResetPasswordAsync("nonexistent@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RequestResetPasswordAsync_UserInactive_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.Status = false;
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));
        _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RequestResetPasswordAsync("test@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RequestResetPasswordAsync_ValidEmail_SendsEmailAndReturnsTrue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));
        _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RequestResetPasswordAsync("test@example.com");

        // Assert
        Assert.True(result);
        Assert.NotNull(user.ResetToken);
        Assert.True(user.ResetTokenExpiry > DateTime.UtcNow);
        _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
        _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Once());
    }
}