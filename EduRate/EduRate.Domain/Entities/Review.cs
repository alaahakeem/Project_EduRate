using System;

namespace EduRate.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? IPAddress { get; set; }
        public bool IsVerified { get; set; }

        public bool IsAnonymous { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int? CenterId { get; set; }
        public Center? Center { get; set; }

        public int? SessionId { get; set; }
        public Session? Session { get; set; }
    }
}
