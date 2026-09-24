using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // هل السنتر أكد حضور الطالب؟
        public bool IsAttended { get; set; } = false;

        // حالة الحجز (Pending, Confirmed, Cancelled)
        public string Status { get; set; } = "Pending";

        // ==========================================
        // العلاقات: طالب وحصة بس (ومن الحصة هنعرف المدرس والسنتر)
        // ==========================================
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public int SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; }
    }
}