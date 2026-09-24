namespace EduRate.Domain.Entities
{
    public class CenterImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public int CenterId { get; set; }
        public Center Center { get; set; } = null!;
    }
}
