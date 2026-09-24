using System.Security.Claims;
using EduRate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EduRate.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? ProfileId
        {
            get
            {
                var claim = User?.FindFirst("ProfileId")?.Value;
                return int.TryParse(claim, out var id) ? id : null;
            }
        }

        public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

        public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
    }
}
