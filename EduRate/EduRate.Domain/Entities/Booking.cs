using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        public bool IsAttended { get; set; } = false;

        // Booking status: Pending, Confirmed, Cancelled
        public string Status { get; set; } = "Pending";

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;

        public int SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; } = null!;
    }
}
