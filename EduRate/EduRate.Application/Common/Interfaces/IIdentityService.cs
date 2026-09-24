namespace EduRate.Application.Common.Interfaces
{
    public class IdentityUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserType { get; set; }
    }

    public class IdentityResult
    {
        public bool Succeeded { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Thin abstraction over ASP.NET Identity's UserManager so the Application layer
    /// (Auth commands) never has to reference EduRate.Infrastructure or ApplicationUser directly.
    /// Implemented in Infrastructure using UserManager&lt;ApplicationUser&gt;.
    /// </summary>
    public interface IIdentityService
    {
        Task<IdentityResult> CreateUserAsync(string email, string userName, string password, string userType);
        Task DeleteUserAsync(string userId);
        Task<IdentityUserDto?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
}
