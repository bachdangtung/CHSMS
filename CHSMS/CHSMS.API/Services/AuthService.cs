using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CHSMS.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly SEP_TestContext _context;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration,
            IEmailService emailService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<TokenPairDto?> AuthenticateAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null)
            {
                return null;
            }
            if (!VerifyPassword(password, user.Password))
            {
                return null;
            }
            if (user.Status == false)
            {
                return new TokenPairDto { AccessToken = "inactive" };
            }

            return await GenerateTokenPair(user);
        }

        private async Task<TokenPairDto> GenerateTokenPair(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiryDays = Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiryInDays"]);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = refreshTokenExpiry;
            await _userRepository.UpdateAsync(user);

            return new TokenPairDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry.Value
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<TokenPairDto?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var tokenParts = accessToken.Split('.');
            if (tokenParts.Length != 3)
            {
                return null;
            }

            var principal = GetPrincipalFromExpiredToken(accessToken);
            var userId = int.Parse(principal.FindFirst("Id")?.Value);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.Status == false || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return null;
            }

            return await GenerateTokenPair(user);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"].Trim())),
                ValidateLifetime = false
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals("HS512", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm");
                }
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Token validation failed: " + ex.Message);
            }
        }

        public async Task<bool> RevokeRefreshToken(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var secretKey = _configuration["Jwt:Key"].Trim();
            var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("name", user.Fullname),
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

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<bool> RequestResetPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;
            if (user.Status == false) return false;

            string resetToken = Guid.NewGuid().ToString();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);

            string resetLink = $"http://127.0.0.1:5500/pages/authen/reset-password.html?token={resetToken}&id={user.UserId}";
            await _emailService.SendAsync(email, "Password Reset Request",
                $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>", true);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByResetTokenAsync(resetPasswordDto);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow || user.Status == false)
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existedUser = await _userRepository.GetByUserNameAsync(createUserDto.UserName);
            if (existedUser != null)
            {
                throw new Exception("Tài khoản đã tồn tại");
            }
            var emailExist = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (emailExist != null)
            {
                throw new Exception("Email đã tồn tại");
            }
            var phoneExist = await _userRepository.GetByPhoneNumber(createUserDto.PhoneNumber);
            if (phoneExist != null)
            {
                throw new Exception("Số điện thoại đã tồn tại");
            }
            var isValidRole = await _roleRepository.RoleExistsAsync(createUserDto.RoleId);
            if (!isValidRole)
            {
                throw new Exception("Vai trò không tồn tại");
            }

            string randomPassword = GenerateRandomPassword(12);

            var user = _mapper.Map<User>(createUserDto);
            user.Status = true;
            user.Password = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            await _userRepository.AddAsync(user);

            string emailBody = $"Tài khoản của bạn đã được tạo:<br><br>" +
                               $"<strong>Tên đăng nhập:</strong> {user.UserName}<br>" +
                               $"<strong>Mật khẩu:</strong> {randomPassword}<br><br>" +
                               $"Vui lòng đăng nhập và thay đổi mật khẩu ngay lập tức.";

            await _emailService.SendAsync(user.Email, "Thông tin tài khoản", emailBody, true);

            return user;
        }

        public async Task<bool> ChangeStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            var activeCount = await _userRepository.CountActiveUser();

            if (user.Status == false && activeCount >= 10)
            {
                throw new Exception("Chỉ được kích hoạt đồng thời 10 tài khoản");
            }

            user.Status = !user.Status;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<IEnumerable<UserListDto>> GetUserListAsync()
        {
            var userList = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserListDto>>(userList);
        }

        public async Task<UserListDto> GetUserProfileAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserListDto>(user);
        }

        public async Task<bool> EditUserProfileAsync(int userId, EditUserProfileDto updatedUser)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            var existEmail = await _userRepository.GetByEmailAsync(updatedUser.Email);
            if (existEmail != null && existEmail.UserId != userId)
            {
                throw new Exception("Email đã tồn tại");
            }
            var phoneExist = await _userRepository.GetByPhoneNumber(updatedUser.PhoneNumber);
            if (phoneExist != null && phoneExist.UserId != userId)
            {
                throw new Exception("   ");
            }
            user.Fullname = updatedUser.Fullname;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Address = updatedUser.Address;
            user.Gender = updatedUser.Gender;
            user.Dob = updatedUser.Dob;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<IEnumerable<UserListDto>> GetUserListAsync(
            string? search, string? gender, bool? status, int? roleId)
        {
            var userList = await _userRepository.GetAllAsync(
                u => (string.IsNullOrEmpty(search) ||
                     u.UserName.Contains(search) ||
                     u.Fullname.Contains(search) ||
                     u.Email.Contains(search)) &&
                     (string.IsNullOrEmpty(gender) || u.Gender == gender) &&
                     (!status.HasValue || u.Status == status.Value) &&
                     (!roleId.HasValue || u.RoleId == roleId)
            );

            return _mapper.Map<IEnumerable<UserListDto>>(userList);
        }



        public string GenerateRandomPassword(int length = 12)
        {
            if (length < 8 || length > 32) throw new ArgumentException("Password length must be between 8 and 32.");

            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "@$!%*?&";
            const string allChars = uppercase + lowercase + digits + specialChars;

            var password = new char[length];
            var random = new Random();

            password[0] = uppercase[random.Next(uppercase.Length)];
            password[1] = lowercase[random.Next(lowercase.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }
    }
}