using System;
using System.ComponentModel.DataAnnotations;

namespace EduRate.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ==========================================
        // العلاقات (Relations) - nullable عشان الإشعار يروح لجهة واحدة
        // ==========================================

        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? CenterId { get; set; }
        public Center? Center { get; set; }
    }
}