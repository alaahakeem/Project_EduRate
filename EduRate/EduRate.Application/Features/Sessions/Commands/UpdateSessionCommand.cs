using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Commands
{
    public class UpdateSessionCommand : IRequest<string>
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
    }

    public class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
    {
        public UpdateSessionCommandValidator()
        {
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
                .WithMessage("موعد انتهاء الحصة يجب أن يكون بعد موعد بدايتها.");
        }
    }

    public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, string>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UpdateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<string> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null) throw new NotFoundException("الحصة غير موجودة.");

            if (session.TeacherId != _currentUser.ProfileId)
                throw new ForbiddenAccessException("لا تملك صلاحية تعديل هذه الحصة.");

            if (request.StartTime < DateTime.Now)
                throw new BadRequestException("لا يمكن تعديل الحصة لموعد قديم.");

            var hasConflict = await _sessionRepository.Query().AnyAsync(s =>
                s.TeacherId == session.TeacherId &&
                s.Id != request.SessionId &&
                s.Status != "Cancelled" &&
                s.StartTime < request.EndTime &&
                request.StartTime < s.EndTime, cancellationToken);

            if (hasConflict)
                throw new BadRequestException("لديك حصة أخرى في التوقيت الجديد.");

            session.Title = request.Title;
            session.StartTime = request.StartTime;
            session.EndTime = request.EndTime;
            session.Price = request.Price;
            session.EducationalStage = request.EducationalStage;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "تم تعديل الحصة بنجاح.";
        }
    }
}
