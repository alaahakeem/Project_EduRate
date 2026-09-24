using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Commands
{
    /// <summary>Student-scoped "mark all read", as originally exposed under StudentController.</summary>
    public class MarkAllNotificationsAsReadCommand : IRequest<string>
    {
        public int StudentId { get; set; }
    }

    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, string>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var unread = await _notificationRepository.Query()
                .Where(n => n.StudentId == request.StudentId && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var notification in unread)
                notification.IsRead = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "All notifications marked as read.";
        }
    }
}
