namespace CHSMS.API.DTOs.User
{
    public class TokenPairDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
