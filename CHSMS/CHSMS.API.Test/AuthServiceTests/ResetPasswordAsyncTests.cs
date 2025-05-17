using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests;

public class ResetPasswordAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AuthService _authService;
    public ResetPasswordAsyncTests()
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
    public async Task ResetPasswordAsync_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var dto = new ResetPasswordDto { Token = "invalid", UserId = 1, NewPassword = "newpassword" };
        _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredToken_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.ResetToken = "token";
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(-1);
        var dto = new ResetPasswordDto { Token = "token", UserId = 1, NewPassword = "newpassword" };
        _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_UserNotExist_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.ResetToken = "token";
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        var dto = new ResetPasswordDto { Token = "token", UserId = -1, NewPassword = "newpassword" };
        _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_InactiveUser_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.Status = false;
        user.ResetToken = "token";
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        var dto = new ResetPasswordDto { Token = "token", UserId = 1, NewPassword = "newpassword" };
        _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_ResetsPasswordAndReturnsTrue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.ResetToken = "token";
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        var dto = new ResetPasswordDto { Token = "token", UserId = 1, NewPassword = "newpassword" };
        _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));

        // Act
        var result = await _authService.ResetPasswordAsync(dto);

        // Assert
        Assert.True(result);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));
        Assert.Null(user.ResetToken);
        Assert.Null(user.ResetTokenExpiry);
        _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
    }
}