namespace EduRate.Application.Features.PromoCodes
{
    public class PromoCodeValidationResultDto
    {
        public string Message { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }
}
