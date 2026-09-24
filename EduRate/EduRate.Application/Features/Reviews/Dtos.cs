namespace EduRate.Application.Features.Reviews
{
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
