using EduRate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EduRate.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task<EduRate.Application.Common.Interfaces.IdentityResult> CreateUserAsync(string email, string userName, string password, string userType)
        {
            var user = new ApplicationUser
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userName,
                UserType = userType
            };

            var result = await _userManager.CreateAsync(user, password);

            return new EduRate.Application.Common.Interfaces.IdentityResult
            {
                Succeeded = result.Succeeded,
                UserId = user.Id,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null) await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityUserDto?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return new IdentityUserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                UserType = user.UserType
            };
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
