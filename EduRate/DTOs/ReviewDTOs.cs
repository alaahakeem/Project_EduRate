using System;
using System.ComponentModel.DataAnnotations;

namespace EduRate.DTOs
{
    // 1. الداتا اللي بنستقبلها وقت التقييم
    public class ReviewCreateDto
    {
        // ❌ شلنا الـ StudentId من هنا عشان هنجيبه من التوكن في الكنترولر للأمان

        // جهات التقييم (بنسمح بواحدة بس منهم)
        public int? TeacherId { get; set; }
        public int? CenterId { get; set; }
        public int? SessionId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون من 1 إلى 5")]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        // هل الطالب عايز التقييم يكون مجهول؟
        public bool IsAnonymous { get; set; }
    }

    // 2. الداتا اللي بنرجعها (تفضل زي ما هي بدون أي تغيير)
    public class ReviewReaddDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool CanEdit { get; set; }
    }
}