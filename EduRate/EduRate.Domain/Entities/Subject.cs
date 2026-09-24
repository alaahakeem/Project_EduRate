using System.Collections.Generic;

namespace EduRate.Domain.Entities
{
    public class Subject
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public EducationalStage EducationalStage { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
