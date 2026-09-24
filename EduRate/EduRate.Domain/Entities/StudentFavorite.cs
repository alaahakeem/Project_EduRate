using System;

namespace EduRate.Domain.Entities
{
    public class StudentFavorite
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? CenterId { get; set; }
        public Center? Center { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
