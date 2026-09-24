using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace EduRate.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public EducationalStage EducationalStage { get; set; }

        // الخصائص الخاصة بالموقع (تم إضافة الإحداثيات لدعم الفلترة بالمسافة)
        public string Governorate { get; set; } // المحافظة (مثال: القاهرة)
        public string Region { get; set; }      // المنطقة (مثال: المعادي)
        public double? Latitude { get; set; }   // خط العرض
        public double? Longitude { get; set; }  // خط الطول

        // 💡 الخصائص الجديدة الخاصة بـ (المحفظة، النقاط، ولي الأمر)\

        [Column(TypeName = "decimal(18,2)")]
        public decimal WalletBalance { get; set; } = 0; // رصيد المحفظة
        public int RewardPoints { get; set; } = 0;      // نقاط المكافآت
        public string? ParentPhoneNumber { get; set; }  // رقم ولي الأمر

        // العلاقات: الطالب ليه حجوزات كتير، وتقييمات كتير
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        // 💡 علاقة المفضلة: السناتر والمدرسين اللي الطالب بيحبهم
        public ICollection<StudentFavorite> Favorites { get; set; } = new List<StudentFavorite>();
    }
}