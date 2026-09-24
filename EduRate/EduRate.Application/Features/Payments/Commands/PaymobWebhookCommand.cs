using System.Text.Json;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Payments.Commands
{
    /// <summary>
    /// Handles the Paymob payment-gateway callback (webhook). Matches the original
    /// PaymentController "callback" endpoint, including its simplified email-matching
    /// approach to identify which student paid.
    /// </summary>
    public class PaymobWebhookCommand : IRequest
    {
        public JsonElement Payload { get; set; }
    }

    public class PaymobWebhookCommandHandler : IRequestHandler<PaymobWebhookCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymobWebhookCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(PaymobWebhookCommand request, CancellationToken cancellationToken)
        {
            var obj = request.Payload.GetProperty("obj");
            var success = obj.GetProperty("success").GetBoolean();

            if (!success) return;

            var amountCents = obj.GetProperty("amount_cents").GetInt32();
            var amountEgp = amountCents / 100m;

            var billingData = obj.GetProperty("order").GetProperty("billing_data");
            var studentEmail = billingData.GetProperty("email").GetString();

            var student = await _studentRepository.Query().FirstOrDefaultAsync(s => s.Email == studentEmail, cancellationToken);
            if (student != null)
            {
                student.WalletBalance += amountEgp;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
