namespace EduRate.Application.Features.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string? UserType { get; set; }
        public int ProfileId { get; set; }
    }
}
