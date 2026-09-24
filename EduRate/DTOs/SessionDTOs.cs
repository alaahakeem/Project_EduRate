using System;

namespace EduRate.DTOs
{
    public class SessionCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
        public int CenterId { get; set; }
        
    }

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

    public class SessionUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
    }
}