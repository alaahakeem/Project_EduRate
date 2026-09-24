namespace EduRate.Application.Features.Centers
{
    public class CenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }

    public class CenterDetailsDto : CenterDto
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
