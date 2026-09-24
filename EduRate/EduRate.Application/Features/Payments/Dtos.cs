namespace EduRate.Application.Features.Payments
{
    public class PaymentResponseDto
    {
        public string RedirectUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
