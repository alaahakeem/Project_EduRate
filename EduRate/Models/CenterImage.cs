namespace EduRate.Models
{
    public class CenterImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; } // عشان نحدد لو دي صورة الغلاف

        // الربط بالسنتر
        public int CenterId { get; set; }
        public Center Center { get; set; }
    }
}