using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Models
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
        public Center Center { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        // العلاقة دي ضرورية جداً عشان نعرف مين حاجز الحصة
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> SessionReviews { get; set; }
    }
}