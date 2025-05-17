using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Tests.AuthServiceTests;

public class GetUserListAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AuthService _authService;
    public GetUserListAsyncTests()
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
    public async Task GetUserListAsync_ReturnsMappedUsers()
    {
        // Arrange
        var users = new List<User> { TestHelper.CreateTestUser() };
        var dtos = new List<UserListDto> { new UserListDto { UserId = 1, Username = "testuser" } };
        _userRepositoryMock.Setup(u => u.GetAllAsync())
            .ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserListDto>>(users))
            .Returns(dtos);

        // Act
        var result = await _authService.GetUserListAsync();

        // Assert
        Assert.Equal(dtos, result);
    }
    /*
        [Fact]
        public async Task GetUserListAsync_WithFilters_ReturnsFilteredUsers()
        {
            // Arrange
            var users = new List<User> { TestHelper.CreateTestUser() };
            var dtos = new List<UserListDto> { new UserListDto { UserId = 1, Username = "testuser" } };
            _userRepositoryMock.Setup(u => u.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserListDto>>(users))
                .Returns(dtos);

            // Act
            var result = await _authService.GetUserListAsync("test", "Male", true, 1);

            // Assert
            Assert.Equal(dtos, result);
        }*/
}