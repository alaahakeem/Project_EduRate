using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Students.Commands
{
    public class RedeemPointsCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public int Points { get; set; }
    }

    public class RedeemPointsCommandValidator : AbstractValidator<RedeemPointsCommand>
    {
        public RedeemPointsCommandValidator()
        {
            RuleFor(x => x.Points).GreaterThan(0);
        }
    }

    public class RedeemPointsCommandHandler : IRequestHandler<RedeemPointsCommand, string>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RedeemPointsCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(RedeemPointsCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            if (student.RewardPoints < request.Points)
                throw new BadRequestException("Not enough reward points.");

            // Business rule preserved exactly: every 100 points = 10 EGP.
            decimal cashValue = (request.Points / 100m) * 10m;

            student.RewardPoints -= request.Points;
            student.WalletBalance += cashValue;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return $"Points redeemed for {cashValue} EGP. New balance: {student.WalletBalance}";
        }
    }
}
