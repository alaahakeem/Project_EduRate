using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Commands
{
    public class CreateSessionCommand : IRequest<SessionReadDto>
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string EducationalStage { get; set; } = string.Empty;
        public int CenterId { get; set; }

        /// <summary>Set by the controller from the caller's JWT (ProfileId claim), not bound from the request body.</summary>
        public int TeacherId { get; set; }
    }

    public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionCommandValidator()
        {
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
                .WithMessage("موعد انتهاء الحصة يجب أن يكون بعد موعد بدايتها.");
        }
    }

    public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, SessionReadDto>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CreateSessionCommandHandler(
            ISessionRepository sessionRepository,
            ICenterRepository centerRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _sessionRepository = sessionRepository;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<SessionReadDto> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.StartTime < DateTime.Now)
                throw new BadRequestException("لا يمكن إنشاء حصة في موعد قديم.");

            var centerExists = await _centerRepository.Query().AnyAsync(c => c.Id == request.CenterId, cancellationToken);
            if (!centerExists)
                throw new BadRequestException("السنتر غير موجود في قاعدة البيانات.");

            var hasConflict = await _sessionRepository.Query().AnyAsync(s =>
                s.TeacherId == request.TeacherId &&
                s.Status != "Cancelled" &&
                s.StartTime < request.EndTime &&
                request.StartTime < s.EndTime, cancellationToken);

            if (hasConflict)
                throw new BadRequestException("لديك حصة أخرى في نفس هذا التوقيت.");

            var newSession = new Session
            {
                Title = request.Title,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Price = request.Price,
                EducationalStage = request.EducationalStage,
                CenterId = request.CenterId,
                TeacherId = request.TeacherId,
                Status = "Available"
            };

            _sessionRepository.Add(newSession);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdSession = await _sessionRepository.Query()
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .FirstOrDefaultAsync(s => s.Id == newSession.Id, cancellationToken);

            if (createdSession != null)
            {
                await _notificationService.SendToCenterAsync(
                    createdSession.CenterId,
                    "حصة جديدة مجدولة \ud83d\udcc5",
                    $"قام المدرس {createdSession.Teacher.Name} بجدولة حصة '{createdSession.Title}' في السنتر الخاص بك يوم {createdSession.StartTime:yyyy-MM-dd}."
                );
            }

            return new SessionReadDto
            {
                Id = createdSession!.Id,
                Title = createdSession.Title,
                StartTime = createdSession.StartTime,
                EndTime = createdSession.EndTime,
                Price = createdSession.Price,
                EducationalStage = createdSession.EducationalStage,
                Status = createdSession.Status,
                CenterName = createdSession.Center.Name,
                TeacherName = createdSession.Teacher.Name
            };
        }
    }
}
