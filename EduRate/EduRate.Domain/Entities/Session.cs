using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Domain.Entities
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";

        public int CenterId { get; set; }
        [ForeignKey("CenterId")]
        public Center Center { get; set; } = null!;

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> SessionReviews { get; set; } = new List<Review>();
    }
}
