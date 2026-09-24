namespace EduRate.Application.Features.Sessions
{
    public class SessionReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }

    public class SessionBookingItemDto
    {
        public int BookingId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
