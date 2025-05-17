using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CHSMS.API.Tests.AuthServiceTests
{
    public class GenerateTokenPairTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public GenerateTokenPairTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup configuration values
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASuperLongSecretKeyWithMoreThanEnoughLengthForHS512");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("30");
            _configurationMock.Setup(c => c["Jwt:RefreshTokenExpiryInDays"]).Returns("7");

            _authService = new AuthService(
                _userRepositoryMock.Object,
                Mock.Of<IRoleRepository>(),
                _configurationMock.Object,
                Mock.Of<IEmailService>(),
                Mock.Of<IMapper>());
        }

        [Fact]
        public async Task GenerateTokenPair_ValidUser_ReturnsTokenPair()
        {
            // Arrange
            var user = TestHelper.CreateTestUser();
            var expectedExpiryDays = 7;
            var expectedExpiry = DateTime.UtcNow.AddDays(expectedExpiryDays);

            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback<User>(u =>
                {
                    // Verify the user was updated correctly
                    Assert.NotNull(u.RefreshToken);
                    Assert.InRange(u.RefreshTokenExpiry.Value,
                        expectedExpiry.AddMinutes(-1),
                        expectedExpiry.AddMinutes(1));
                });

            // Act
            var result = await _authService.GenerateTokenPair(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.AccessToken);
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.InRange(result.RefreshTokenExpiry,
                expectedExpiry.AddMinutes(-1),
                expectedExpiry.AddMinutes(1));
            Assert.Equal("TestIssuer", token.Issuer);
            Assert.Equal("TestAudience", token.Audiences.First());
            Assert.Contains(token.Claims, c => c.Type == "Id" && c.Value == user.UserId.ToString());
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        }
    }
}