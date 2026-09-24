using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace EduRate.Application.Features.Payments.Commands
{
    /// <summary>
    /// Paymob-gateway wallet top-up. Matches the original PaymentController "charge" endpoint
    /// (distinct from Students.ChargeWalletCommand, which is the direct/self-service top-up).
    /// </summary>
    public class ChargeWalletViaGatewayCommand : IRequest<PaymentResponseDto>
    {
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class ChargeWalletViaGatewayCommandHandler : IRequestHandler<ChargeWalletViaGatewayCommand, PaymentResponseDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;

        public ChargeWalletViaGatewayCommandHandler(IStudentRepository studentRepository, IPaymobService paymobService, IConfiguration configuration)
        {
            _studentRepository = studentRepository;
            _paymobService = paymobService;
            _configuration = configuration;
        }

        public async Task<PaymentResponseDto> Handle(ChargeWalletViaGatewayCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("الطالب غير موجود.");

            if (request.Amount <= 0) throw new BadRequestException("المبلغ يجب أن يكون أكبر من الصفر.");

            var authToken = await _paymobService.GetAuthTokenAsync();
            var orderId = await _paymobService.CreateOrderAsync(authToken, request.Amount);

            var paymentKey = await _paymobService.GetPaymentKeyAsync(
                authToken,
                orderId,
                request.Amount,
                request.Email ?? student.Email,
                request.FirstName ?? student.Name,
                request.LastName ?? "Student",
                request.PhoneNumber ?? string.Empty,
                request.PaymentMethod ?? string.Empty
            );

            var iframeId = _configuration["Paymob:IframeId"];
            var iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

            return new PaymentResponseDto
            {
                RedirectUrl = iframeUrl,
                Message = "تم تجهيز رابط الدفع بنجاح. يرجى التوجه للرابط لإتمام العملية."
            };
        }
    }
}
