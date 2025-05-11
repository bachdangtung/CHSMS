using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Tests.AuthServiceTests;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Test.AuthServiceTests;

public class RefreshTokenAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<SEP_TestContext> _contextMock;
    private readonly AuthService _authService;
    public RefreshTokenAsyncTests()
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
        public async Task RefreshTokenAsync_InvalidTokenFormat_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalidToken";

            // Act
            var result = await _authService.RefreshTokenAsync(invalidToken, "refreshtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var invalidToken = TestHelper.GenerateJwtTokenForTest(1, null, null, "This Is A Super Wrong Secret Key With More Than Enough Length For HS512");

            // Act & Assert
            await Assert.ThrowsAnyAsync<SecurityTokenException>(() =>
                _authService.RefreshTokenAsync(invalidToken, "refreshtoken"));
        }

        [Fact]
        public async Task RefreshTokenAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var token = TestHelper.GenerateJwtTokenForTest(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.RefreshTokenAsync(token, "refreshtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.Status = false;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = TestHelper.GenerateJwtTokenForTest(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.RefreshTokenAsync(token, "refreshtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_InvalidRefreshToken_ReturnsNull()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.RefreshToken = "differentrefreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = TestHelper.GenerateJwtTokenForTest(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.RefreshTokenAsync(token, "refreshtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ExpiredRefreshToken_ReturnsNull()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1);
            var token = TestHelper.GenerateJwtTokenForTest(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.RefreshTokenAsync(token, "refreshtoken");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokenPair()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = TestHelper.GenerateJwtTokenForTest(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.RefreshTokenAsync(token, "refreshtoken");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiry > DateTime.UtcNow);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
}