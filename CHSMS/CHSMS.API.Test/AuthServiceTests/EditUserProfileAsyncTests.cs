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

public class EditUserProfileAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _authService;
    public EditUserProfileAsyncTests()
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
    public async Task EditUserProfileAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var dto = new EditUserProfileDto();
        _userRepositoryMock.Setup(u => u.GetByIdAsync(999))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.EditUserProfileAsync(999, dto));
    }

    [Fact]
    public async Task EditUserProfileAsync_ValidInput_UpdatesUser()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        var dto = new EditUserProfileDto
        {
            Fullname = "Updated Name",
            Email = "updated@example.com",
            PhoneNumber = "0987654321",
            Address = "Updated Address",
            Gender = "Female",
            Dob = new DateTime(1995, 5, 5)
        };
        _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()));

        // Act
        var result = await _authService.EditUserProfileAsync(1, dto);

        // Assert
        Assert.True(result);
        Assert.Equal(dto.Fullname, user.Fullname);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.PhoneNumber, user.PhoneNumber);
        Assert.Equal(dto.Address, user.Address);
        Assert.Equal(dto.Gender, user.Gender);
        Assert.Equal(dto.Dob, user.Dob);
        _userRepositoryMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once());
    }
}