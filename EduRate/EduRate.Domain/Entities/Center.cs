using System.Collections.Generic;

namespace EduRate.Domain.Entities
{
    public class Center
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Links the Center profile to its Identity login account
        public string? UserId { get; set; }

        public ICollection<TeacherCenter> TeacherCenters { get; set; } = new List<TeacherCenter>();

        public ICollection<Review> CenterReviews { get; set; } = new List<Review>();

        public ICollection<CenterImage> CenterImages { get; set; } = new List<CenterImage>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<StudentFavorite> FavoritedByStudents { get; set; } = new List<StudentFavorite>();

        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
    }
}
