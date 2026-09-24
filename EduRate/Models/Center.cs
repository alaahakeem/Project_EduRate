using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EduRate.Models
{
    public class Center
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool IsVerified { get; set; }

        // الخصائص الخاصة بالخريطة (خليناها Nullable عشان لو سنتر لسه متحددش موقعه)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // 💡 التعديل هنا: إضافة حقل UserId لربط السنتر بحساب الـ Login
        public string? UserId { get; set; }

        // العلاقة مع جدول الوسيط (TeacherCenter)
        public ICollection<TeacherCenter> TeacherCenters { get; set; }

        public ICollection<Review> CenterReviews { get; set; }

        // تم تصحيح الاسم ليتطابق مع الـ DbContext والكنترولر
        public ICollection<CenterImage> CenterImages { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        // 💡 إضافة العلاقة الجديدة: الطلاب اللي حاطين السنتر ده في المفضلة
        public ICollection<StudentFavorite> FavoritedByStudents { get; set; } = new List<StudentFavorite>();

        public ICollection<PromoCode> PromoCodes { get; set; }
    }
}