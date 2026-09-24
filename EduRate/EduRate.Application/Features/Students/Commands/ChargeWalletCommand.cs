using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Students.Commands
{
    /// <summary>
    /// Direct wallet top-up (self-service), distinct from the Paymob-gateway top-up in the
    /// Payments feature. Matches StudentController's original "my-wallet/charge" endpoint.
    /// </summary>
    public class ChargeWalletCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ChargeWalletCommandValidator : AbstractValidator<ChargeWalletCommand>
    {
        public ChargeWalletCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }

    public class ChargeWalletCommandHandler : IRequestHandler<ChargeWalletCommand, string>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChargeWalletCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(ChargeWalletCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            student.WalletBalance += request.Amount;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return $"Wallet charged successfully. New balance: {student.WalletBalance}";
        }
    }
}
