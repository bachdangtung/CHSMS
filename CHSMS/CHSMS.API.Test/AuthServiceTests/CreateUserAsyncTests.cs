using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Tests.AuthServiceTests;

public class CreateUserAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AuthService _authService;
    public CreateUserAsyncTests()
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
    public async Task CreateUserAsync_ValidInput_CreatesUserAndSendsEmail()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            UserName = "newuser",
            Email = "new@example.com",
            RoleId = 1,
            Fullname = "New User"
        };
        var user = new User
        {
            UserName = "newuser",
            Email = "new@example.com",
            RoleId = 1,
            Fullname = "New User",
            Status = true
        };
        _userRepositoryMock.Setup(u => u.GetByUserNameAsync("newuser"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByPhoneNumber("0987654321"))
            .ReturnsAsync((User)null);
        _roleRepositoryMock.Setup(r => r.RoleExistsAsync(1))
            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _userRepositoryMock.Setup(u => u.AddAsync(It.IsAny<User>()));
        _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.CreateUserAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.NotNull(result.Password);
        _userRepositoryMock.Verify(u => u.AddAsync(It.IsAny<User>()), Times.Once());
        _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Once());
    }

    [Fact]
    public async Task CreateUserAsync_UsernameExists_ThrowsException()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "testuser", Email = "new@example.com", RoleId = 1 };
        _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
            .ReturnsAsync(TestHelper.CreateTestUser());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
    }

    [Fact]
    public async Task CreateUserAsync_EmailExists_ThrowsException()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "newuser", Email = "test@example.com", RoleId = 1 };
        _userRepositoryMock.Setup(u => u.GetByUserNameAsync("newuser"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(TestHelper.CreateTestUser());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
    }

    [Fact]
    public async Task CreateUserAsync_PhoneExists_ThrowsExceptionWithCorrectMessage()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "newuser", Email = "new@example.com", PhoneNumber = "0123456789", RoleId = 1 };
        _userRepositoryMock.Setup(u => u.GetByUserNameAsync("newuser"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByPhoneNumber("0123456789"))
            .ReturnsAsync(TestHelper.CreateTestUser());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
        Assert.Equal("Số điện thoại đã tồn tại", ex.Message);
    }

    [Fact]
    public async Task CreateUserAsync_InvalidRole_ThrowsException()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "newuser", Email = "new@example.com", RoleId = -1 };
        _userRepositoryMock.Setup(u => u.GetByUserNameAsync("newuser"))
            .ReturnsAsync((User)null);
        _userRepositoryMock.Setup(u => u.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User)null);
        _roleRepositoryMock.Setup(r => r.RoleExistsAsync(-1))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
    }

}