using System.Collections.Generic;
// شلنا السطر بتاع الـ using الغلط خالص

namespace EduRate.Models
{
    public class Subject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // التعديل هنا: النوع هو اسم الـ Enum نفسه
        public EducationalStage EducationalStage { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}