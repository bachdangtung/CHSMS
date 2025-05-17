using AutoMapper;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;

namespace CHSMS.API.Test.AuthServiceTests;

public class GenerateJwtTokenTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _authService;
    public GenerateJwtTokenTest()
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
    public void GenerateJwtToken_ValidUser_GeneratesValidToken()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == "email").Value);
        Assert.Equal(user.Fullname, jwtToken.Claims.First(c => c.Type == "name").Value);
        Assert.Equal(user.UserId.ToString(), jwtToken.Claims.First(c => c.Type == "Id").Value);
        Assert.Equal(user.Role.RoleName, jwtToken.Claims.First(c => c.Type == "role").Value);
        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
    }
}