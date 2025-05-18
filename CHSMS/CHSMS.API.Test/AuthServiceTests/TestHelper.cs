using CHSMS.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CHSMS.API.Tests.AuthServiceTests;

public class TestHelper
{
    public TestHelper()
    {

    }
    public static User CreateTestUser(int id = 1, string roleName = "Doctor")
    {
        return new User
        {
            UserId = id,
            UserName = "testuser",
            Email = "test@example.com",
            Fullname = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123@"),
            Status = true,
            Role = new Role { RoleName = roleName },
            RoleId = 1,
            PhoneNumber = "0123456789",
            Address = "Test Address",
            Gender = "Male",
            Dob = new DateTime(1990, 1, 1)
        };
    }

    public static string GenerateJwtTokenForTest(int userId, DateTime? notBefore = null, DateTime? expires = null, string? wrongKey = null)
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