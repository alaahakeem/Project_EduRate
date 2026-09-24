using System.Collections.Generic;

namespace EduRate.Domain.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public double TrustScore { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string DemoVideoUrl { get; set; } = string.Empty;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public ICollection<TeacherCenter> TeacherCenters { get; set; } = new List<TeacherCenter>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<StudentFavorite> Favorites { get; set; } = new List<StudentFavorite>();
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
