using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Commands
{
    public class CancelSessionCommand : IRequest<string>
    {
        public int SessionId { get; set; }
    }

    public class CancelSessionCommandHandler : IRequestHandler<CancelSessionCommand, string>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationService _notificationService;

        public CancelSessionCommandHandler(
            ISessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            INotificationService notificationService)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.Query()
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.Student)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

            if (session == null) throw new NotFoundException("الحصة غير موجودة.");

            if (session.TeacherId != _currentUser.ProfileId)
                throw new ForbiddenAccessException("لا تملك صلاحية إلغاء هذه الحصة.");

            if (session.Status == "Cancelled")
                throw new BadRequestException("هذه الحصة ملغية بالفعل.");

            session.Status = "Cancelled";

            if (session.Bookings != null)
            {
                foreach (var booking in session.Bookings.Where(b => b.Status != "Cancelled"))
                {
                    booking.Status = "Cancelled";

                    if (booking.Student != null)
                    {
                        booking.Student.WalletBalance += session.Price;

                        await _notificationService.SendToStudentAsync(
                            booking.Student.Id,
                            "إلغاء حصة واسترداد أموال \u26a0\ufe0f",
                            $"نعتذر، تم إلغاء حصة '{session.Title}'. تمت إعادة مبلغ {session.Price} إلى محفظتك."
                        );
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "تم إلغاء الحصة، واسترداد الأموال للطلاب في محافظهم بنجاح.";
        }
    }
}
