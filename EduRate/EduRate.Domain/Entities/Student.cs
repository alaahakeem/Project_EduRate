using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public EducationalStage EducationalStage { get; set; }

        public string Governorate { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal WalletBalance { get; set; } = 0;
        public int RewardPoints { get; set; } = 0;
        public string? ParentPhoneNumber { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<StudentFavorite> Favorites { get; set; } = new List<StudentFavorite>();
    }
}
