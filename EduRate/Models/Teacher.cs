namespace EduRate.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 💡 مسحنا السطر بتاع: public string Subject { get; set; } من هنا

        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public double TrustScore { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string DemoVideoUrl { get; set; } = string.Empty; // لينك الفيديو التجريبي

        public ICollection<Review> Reviews { get; set; }  //parent
        public ICollection<Message> Messages { get; set; }

        // العلاقة مع جدول الوسيط (TeacherCenter)
        public ICollection<TeacherCenter> TeacherCenters { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<StudentFavorite> Favorites { get; set; } = new List<StudentFavorite>();
        public ICollection<PromoCode> PromoCodes { get; set; }

        // 💡 ربط المدرس بجدول المواد (تم تعديل الاسم للمفرد)
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}