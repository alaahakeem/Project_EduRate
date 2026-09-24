namespace EduRate.Application.Features.Teachers
{
    public class TeacherReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public double TrustScore { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string DemoVideoUrl { get; set; } = string.Empty;
    }

    public class TeacherStatsDto
    {
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public double TrustScore { get; set; }
    }

    public class BookingReadDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string StudentName { get; set; } = string.Empty;
        // Never actually set by the original mapping, but kept here (defaults to false)
        // to preserve the exact original JSON response shape.
        public bool IsConfirmed { get; set; }
    }
}
