using System.ComponentModel.DataAnnotations;
using EduRate.Models;

namespace EduRate.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "UserType is required (e.g., Student, Teacher, Center)")]
        public string UserType { get; set; }

        // 💡 حقول خاصة بالطالب (المرحلة، المحافظة، والمنطقة)
        public int? EducationalStage { get; set; }
        public string? Governorate { get; set; }
        public string? Region { get; set; }

        public int? SubjectId { get; set; }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}