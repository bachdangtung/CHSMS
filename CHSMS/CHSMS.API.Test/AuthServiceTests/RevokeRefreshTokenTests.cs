using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests;

public class RevokeRefreshTokenTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AuthService _authService;
    public RevokeRefreshTokenTests()
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
    public async Task RevokeRefreshToken_UserNotFound_ReturnsFalse()
    {
        // Arrange
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.RevokeRefreshToken(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RevokeRefreshToken_ValidUser_RevokesTokenAndReturnsTrue()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        user.RefreshToken = "refreshtoken";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));

        // Act
        var result = await _authService.RevokeRefreshToken(1);

        // Assert
        Assert.True(result);
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiry);
        _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
    }

}