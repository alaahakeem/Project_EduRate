using Microsoft.AspNetCore.Identity;

namespace EduRate.Models
{
    // الكلاس ده بيورث من IdentityUser اللي فيه جاهز (الإيميل، الباسورد المتشفر، اليوزر نيم)
    public class ApplicationUser : IdentityUser
    {
        // ممكن نحدد نوع اليوزر (Student, Teacher, Center, Admin)
        public string UserType { get; set; }
    }
}