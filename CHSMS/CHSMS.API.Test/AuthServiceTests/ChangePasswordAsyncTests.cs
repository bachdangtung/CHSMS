using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests;

public class ChangePasswordAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _authService;

    public ChangePasswordAsyncTests()
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
    public async Task ChangePasswordAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.ChangePasswordAsync(1, "oldpassword", "newpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_IncorrectOldPassword_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ChangePasswordAsync(1, "wrongpassword", "newpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidInput_ChangesPasswordAndReturnsTrue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Mock UpdateAsync (which handles saving internally)
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ChangePasswordAsync(1, "password", "newpassword");

        // Assert
        Assert.True(result);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));

        // Verify UpdateAsync was called (no need to check SaveChanges)
        _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
    }
}
