namespace EduRate.Application.Features.Students
{
    public class StudentProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EducationalStage { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public decimal WalletBalance { get; set; }
        public int RewardPoints { get; set; }
    }

    public class WalletInfoDto
    {
        public decimal WalletBalance { get; set; }
        public int RewardPoints { get; set; }
    }

    public class StudentBookingDto
    {
        public int BookingId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsAttended { get; set; }
    }

    public class StudentReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? TargetName { get; set; }
    }

    public class StudentNotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StudentFavoriteDto
    {
        public int FavoriteId { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int? CenterId { get; set; }
        public string? CenterName { get; set; }
    }
}
