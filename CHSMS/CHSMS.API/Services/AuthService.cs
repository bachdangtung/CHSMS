using AutoMapper;
using CHSMS.API.Models;
using CHSMS.API.Services.Interfaces;
using CHSMS.API.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CHSMS.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var secretKey = _configuration["Jwt:Key"];
            var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Id", user.UserId.ToString()),
    };

            if (user.Role != null)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha512Signature),
                Issuer = issuer,
                Audience = audience,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }

        // Hash the password before storing it
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verify password during login
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        /*        // Request Reset Password (Generate Token & Send Email)
                public async Task<bool> RequestResetPasswordAsync(string email)
                {
                    var user = await _unitOfWork.Users.GetByEmailAsync(email);
                    if (user == null) return false;

                    string resetToken = Guid.NewGuid().ToString();
                    user.ResetToken = resetToken;
                    user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveAsync();

                    await _emailService.SendResetPasswordEmail(email, resetToken);
                    return true;
                }

                // Reset Password (Verify Token & Set New Password)
                public async Task<bool> ResetPasswordAsync(string token, string newPassword)
                {
                    var user = await _unitOfWork.UserRepository.GetByResetTokenAsync(token);
                    if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                        return false;

                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    user.ResetToken = null;
                    user.ResetTokenExpiry = null;

                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveAsync();
                    return true;
                }*/

        // Change Password (Verify Old Password & Update)
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
