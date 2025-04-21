using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace CHSMS.API.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
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

        private User CreateTestUser(int id = 1, string roleName = "User")
        {
            return new User
            {
                UserId = id,
                UserName = "testuser",
                Email = "test@example.com",
                Fullname = "Test User",
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                Status = true,
                Role = new Role { RoleName = roleName },
                RoleId = 1,
                PhoneNumber = "1234567890",
                Address = "Test Address",
                Gender = "Male",
                Dob = new DateTime(1990, 1, 1)
            };
        }

        // Tests for AuthenticateAsync
        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("nonexistent"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync("nonexistent", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_IncorrectPassword_ReturnsNull()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_InactiveUser_ReturnsInactiveToken()
        {
            // Arrange
            var user = CreateTestUser();
            user.Status = false;
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("inactive", result.AccessToken);
            Assert.Null(result.RefreshToken);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTokenPair()
        {
            // Arrange
            var user = CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiry > DateTime.UtcNow);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for RefreshTokenAsync
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
            var invalidToken = GenerateJwtTokenForTest(1, null, null, "This Is A Super Wrong Secret Key With More Than Enough Length For HS512");

            // Act & Assert
            await Assert.ThrowsAnyAsync<SecurityTokenException>(() =>
                _authService.RefreshTokenAsync(invalidToken, "refreshtoken"));
        }

        [Fact]
        public async Task RefreshTokenAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var token = GenerateJwtTokenForTest(1);
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
            var user = CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.Status = false;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = GenerateJwtTokenForTest(1);
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
            var user = CreateTestUser();
            user.RefreshToken = "differentrefreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = GenerateJwtTokenForTest(1);
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
            var user = CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1);
            var token = GenerateJwtTokenForTest(1);
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
            var user = CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            var token = GenerateJwtTokenForTest(1);
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

        // Tests for RevokeRefreshToken
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
            var user = CreateTestUser();
            user.RefreshToken = "refreshtoken";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.RevokeRefreshToken(1);

            // Assert
            Assert.True(result);
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for GenerateJwtToken
        [Fact]
        public void GenerateJwtToken_ValidUser_GeneratesValidToken()
        {
            // Arrange
            var user = CreateTestUser();

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

        // Tests for HashPassword and VerifyPassword
        [Fact]
        public void HashPassword_CreatesValidHash()
        {
            // Arrange
            var password = "password123";

            // Act
            var hash = AuthService.HashPassword(password);

            // Assert
            Assert.True(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "password123";
            var hash = AuthService.HashPassword(password);

            // Act
            var result = AuthService.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "password123";
            var hash = AuthService.HashPassword(password);

            // Act
            var result = AuthService.VerifyPassword("wrongpassword", hash);

            // Assert
            Assert.False(result);
        }

        // Tests for ChangePasswordAsync
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
            var user = CreateTestUser();
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
            var user = CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.ChangePasswordAsync(1, "password", "newpassword");

            // Assert
            Assert.True(result);
            Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for RequestResetPasswordAsync
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
            var user = CreateTestUser();
            user.Status = false;
            _userRepositoryMock.Setup(u => u.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
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
            var user = CreateTestUser();
            _userRepositoryMock.Setup(u => u.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RequestResetPasswordAsync("test@example.com");

            // Assert
            Assert.True(result);
            Assert.NotNull(user.ResetToken);
            Assert.True(user.ResetTokenExpiry > DateTime.UtcNow);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
            _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Once());
        }

        // Tests for ResetPasswordAsync
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
            var user = CreateTestUser();
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
            var user = CreateTestUser();
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
            var user = CreateTestUser();
            user.Status = false;
            user.ResetToken = "token";
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            var dto = new ResetPasswordDto { Token = "token", UserId = 1, NewPassword = "newpassword" };
            _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.ResetPasswordAsync(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ValidToken_ResetsPasswordAndReturnsTrue()
        {
            // Arrange
            var user = CreateTestUser();
            user.ResetToken = "token";
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            var dto = new ResetPasswordDto { Token = "token", UserId = 1, NewPassword = "newpassword" };
            _userRepositoryMock.Setup(u => u.GetByResetTokenAsync(dto))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.ResetPasswordAsync(dto);

            // Assert
            Assert.True(result);
            Assert.True(BCrypt.Net.BCrypt.Verify("newpassword", user.Password));
            Assert.Null(user.ResetToken);
            Assert.Null(user.ResetTokenExpiry);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for CreateUserAsync
        [Fact]
        public async Task CreateUserAsync_UsernameExists_ThrowsException()
        {
            // Arrange
            var dto = new CreateUserDto { UserName = "testuser", Email = "new@example.com", RoleId = 1 };
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("testuser"))
                .ReturnsAsync(CreateTestUser());

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
                .ReturnsAsync(CreateTestUser());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
        }

        [Fact]
        public async Task CreateUserAsync_InvalidRole_ThrowsException()
        {
            // Arrange
            var dto = new CreateUserDto { UserName = "newuser", Email = "new@example.com", RoleId = 1 };
            _userRepositoryMock.Setup(u => u.GetByUserNameAsync("newuser"))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(u => u.GetByEmailAsync("new@example.com"))
                .ReturnsAsync((User)null);
            _roleRepositoryMock.Setup(r => r.RoleExistsAsync(1))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.CreateUserAsync(dto));
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
            _roleRepositoryMock.Setup(r => r.RoleExistsAsync(1))
                .ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userRepositoryMock.Setup(u => u.Add(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.CreateUserAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Status);
            Assert.NotNull(result.Password);
            _userRepositoryMock.Verify(u => u.Add(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
            _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Once());
        }

        // Tests for ChangeStatusAsync
        [Fact]
        public async Task ChangeStatusAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _userRepositoryMock.Setup(u => u.GetByIdAsync(-1))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.ChangeStatusAsync(-1));
        }

        [Fact]
        public async Task ChangeStatusAsync_ValidUser_TogglesStatus()
        {
            // Arrange
            var user = CreateTestUser();
            user.Status = true;
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.ChangeStatusAsync(1);

            // Assert
            Assert.True(result);
            Assert.False(user.Status);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for GetUserListAsync (simple)
        [Fact]
        public async Task GetUserListAsync_ReturnsMappedUsers()
        {
            // Arrange
            var users = new List<User> { CreateTestUser() };
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

        // Tests for GetUserProfileAsync
        [Fact]
        public async Task GetUserProfileAsync_ReturnsMappedUser()
        {
            // Arrange
            var user = CreateTestUser();
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

        // Tests for EditUserProfileAsync
        [Fact]
        public async Task EditUserProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var dto = new EditUserProfileDto();
            _userRepositoryMock.Setup(u => u.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.EditUserProfileAsync(1, dto));
        }

        [Fact]
        public async Task EditUserProfileAsync_ValidInput_UpdatesUser()
        {
            // Arrange
            var user = CreateTestUser();
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
            _userRepositoryMock.Setup(u => u.Update(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

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
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        // Tests for GetUserListAsync (filtered)
        [Fact]
        public async Task GetUserListAsync_WithFilters_ReturnsFilteredUsers()
        {
            // Arrange
            var users = new List<User> { CreateTestUser() };
            var dtos = new List<UserListDto> { new UserListDto { UserId = 1, Username = "testuser" } };
            _userRepositoryMock.Setup(u => u.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserListDto>>(users))
                .Returns(dtos);

            // Act
            var result = await _authService.GetUserListAsync("test", "Male", true, 1);

            // Assert
            Assert.Equal(dtos, result);
        }

        // Tests for GenerateRandomPassword
        [Fact]
        public void GenerateRandomPassword_ValidLength_GeneratesPassword()
        {
            // Act
            var password = _authService.GenerateRandomPassword(12);

            // Assert
            Assert.Equal(12, password.Length);
            Assert.Contains(password, c => char.IsUpper(c));
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsDigit(c));
            Assert.Contains(password, c => "@$!%*?&".Contains(c));
        }

        [Theory]
        [InlineData(7)]
        [InlineData(33)]
        public void GenerateRandomPassword_InvalidLength_ThrowsArgumentException(int length)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _authService.GenerateRandomPassword(length));
        }

        // Helper method to generate a JWT token for testing
        private string GenerateJwtTokenForTest(int userId, DateTime? notBefore = null, DateTime? expires = null, string? wrongKey = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("This Is A Super Long Secret Key With More Than Enough Length For HS512");
            if (!string.IsNullOrEmpty(wrongKey))
            {
                key = Encoding.UTF8.GetBytes(wrongKey);
            }
            var claims = new List<Claim>
            {
                new Claim("Id", userId.ToString())
            };
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = notBefore ?? now.AddMinutes(-35),
                Expires = expires ?? now.AddMinutes(-30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}