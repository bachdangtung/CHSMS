using System.IdentityModel.Tokens.Jwt;
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

public class GetUserProfileAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<SEP_TestContext> _contextMock;
    private readonly AuthService _authService;
    public GetUserProfileAsyncTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _emailServiceMock = new Mock<IEmailService>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<SEP_TestContext>();

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
            _mapperMock.Object,
            _contextMock.Object);
    }
    [Fact]
    public async Task GetUserProfileAsync_ReturnsMappedUser()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        var dto = new UserListDto { UserId = 1, Username = "testuser" };
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserListDto>(user))
            .Returns(dto);

        // Act
        var result = await _authService.GetUserProfileAsync(1);

        // Assert
        Assert.Equal(dto, result);
    }
}