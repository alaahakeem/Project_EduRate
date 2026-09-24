using System;
using System.ComponentModel.DataAnnotations;

namespace EduRate.DTOs
{
    // ================= CRUD DTOs =================
    public class CenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsVerified { get; set; }
    }

    public class CenterDetailsDto : CenterDto
    {
        // 💡 التعديل: خليناهم يقبلوا Null عشان يطابقوا الموديل
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class CenterCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        // 💡 التعديل: خليناهم يقبلوا Null
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class CenterUpdateDto : CenterCreateDto
    {
    }

    // ================= Teachers in Center DTOs =================
    public class CenterTeacherDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string Subject { get; set; }
        public decimal Price { get; set; }
        public decimal ProfitPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    public class AddTeacherToCenterDto
    {
        [Required]
        public int TeacherId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal ProfitPercentage { get; set; }
    }

    // ================= Reviews DTOs =================
    public class CenterReviewDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public class CenterReviewCreateDto
    {
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    // ================= Schedule & Stats DTOs =================
    public class CenterScheduleDto
    {
        public int SessionId { get; set; }
        public string TeacherName { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class CenterStatsDto
    {
        public int TotalTeachers { get; set; }
        public int TotalStudentsBooked { get; set; }
        public double AverageRating { get; set; }
    }

    public class CenterImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } // لينك الصورة
        public bool IsMain { get; set; } // هل دي الصورة الرئيسية للسنتر؟
    }
}