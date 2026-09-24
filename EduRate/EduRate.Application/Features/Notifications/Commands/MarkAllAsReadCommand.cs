using EduRate.Application.Common.Interfaces;
using EduRate.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Notifications.Commands
{
    public class MarkAllAsReadCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, string>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllAsReadCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            var query = NotificationQueryHelper.ForUser(_notificationRepository, request.Role, request.UserId);
            var unread = await query.Where(n => !n.IsRead).ToListAsync(cancellationToken);

            if (!unread.Any()) return "لا يوجد إشعارات جديدة";

            foreach (var notification in unread)
                notification.IsRead = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "تم تحديد جميع الإشعارات كمقروءة";
        }
    }
}
