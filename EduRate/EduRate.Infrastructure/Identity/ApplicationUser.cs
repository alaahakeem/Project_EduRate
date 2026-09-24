using Microsoft.AspNetCore.Identity;

namespace EduRate.Infrastructure.Identity
{
    // Extends IdentityUser with the app-specific "which kind of profile" marker
    // (Student, Teacher, Center, Admin), exactly as the original EduRate.Models.ApplicationUser did.
    public class ApplicationUser : IdentityUser
    {
        public string? UserType { get; set; }
    }
}
