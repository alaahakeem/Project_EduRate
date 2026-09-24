namespace EduRate.Application.Features.Bookings
{
    public class BookingReaddDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsAttended { get; set; }
        public string Status { get; set; } = string.Empty;

        public int SessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        public decimal SessionPrice { get; set; }

        public string TeacherName { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
}
