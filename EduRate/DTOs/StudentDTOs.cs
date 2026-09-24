using System;
using EduRate.Models;

namespace EduRate.DTOs
{
    // === 1. Profile DTOs ===
    public class StudentProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EducationalStage { get; set; }
        public string Governorate { get; set; }
        public string Region { get; set; }
        public string ParentPhoneNumber { get; set; }
        public decimal WalletBalance { get; set; }
        public int RewardPoints { get; set; }
    }

    public class UpdateStudentProfileDto
    {
        public string Name { get; set; }
        public string ParentPhoneNumber { get; set; }
        public EducationalStage EducationalStage { get; set; }
    }

    // === 2. Location DTO ===
    public class UpdateLocationDto
    {
        public string Governorate { get; set; }
        public string Region { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    // === 3. Financial DTOs ===
    public class WalletInfoDto
    {
        public decimal WalletBalance { get; set; }
        public int RewardPoints { get; set; }
    }

    public class ChargeWalletDto
    {
        public decimal Amount { get; set; }
    }

    public class RedeemPointsDto
    {
        public int Points { get; set; }
    }

    // === 4. Booking DTO ===
    public class StudentBookingDto
    {
        public int BookingId { get; set; }
        public string SessionTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public bool IsAttended { get; set; }
    }

    // === 5. Review DTO ===
    public class StudentReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetName { get; set; } // اسم المدرس أو السنتر اللي اتقيم
    }

    // === 6. Notification DTO ===
    public class StudentNotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // === 7. Favorite DTOs ===
    public class StudentFavoriteDto
    {
        public int FavoriteId { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int? CenterId { get; set; }
        public string CenterName { get; set; }
    }

    public class AddFavoriteDto
    {
        public int? TeacherId { get; set; }
        public int? CenterId { get; set; }
    }
    namespace EduRate.DTOs
    {
        public class StudentProfileDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string EducationalStage { get; set; }
            public string Governorate { get; set; }
            public string Region { get; set; }
            public double WalletBalance { get; set; }
            public int RewardPoints { get; set; }
        }
    }
}