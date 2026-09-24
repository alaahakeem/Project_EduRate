namespace EduRate.DTOs
{
    // الداتا اللي بنعرضها
    public class SubjectDTOs
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EducationalStage { get; set; } = string.Empty;
    }

    // الداتا اللي الأدمن بيبعتها عشان يضيف مادة جديدة
    public class SubjectCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string EducationalStage { get; set; } = string.Empty;
    }
}