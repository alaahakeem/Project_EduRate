using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Notifications.Queries
{
    public class GetMyUnreadCountQuery : IRequest<int>
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class GetMyUnreadCountQueryHandler : IRequestHandler<GetMyUnreadCountQuery, int>
    {
        private readonly INotificationRepository _notificationRepository;
        public GetMyUnreadCountQueryHandler(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

        public Task<int> Handle(GetMyUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var query = NotificationQueryHelper.ForUser(_notificationRepository, request.Role, request.UserId);
            return query.CountAsync(n => !n.IsRead, cancellationToken);
        }
    }
}
