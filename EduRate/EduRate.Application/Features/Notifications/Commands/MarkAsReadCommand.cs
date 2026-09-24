using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Notifications.Commands
{
    public class MarkAsReadCommand : IRequest<string>
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, string>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAsReadCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var query = NotificationQueryHelper.ForUser(_notificationRepository, request.Role, request.UserId);
            var notification = await query.FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

            if (notification == null) throw new NotFoundException("الإشعار غير موجود أو لا تملك صلاحية تعديله.");

            notification.IsRead = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "تم تحديد الإشعار كمقروء";
        }
    }
}
