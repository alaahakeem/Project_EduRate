using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Notifications.Queries
{
    /// <summary>
    /// The generic, role-aware notification feed exposed by NotificationsController
    /// (distinct from Students.GetMyNotificationsQuery, which is the student-only one
    /// under StudentController).
    /// </summary>
    public class GetMyNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        public GetMyNotificationsQueryHandler(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

        public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var query = NotificationQueryHelper.ForUser(_notificationRepository, request.Role, request.UserId);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }

    /// <summary>Small internal helper mirroring the original NotificationsController's
    /// GetUserNotificationsQuery(role, userId) filter, shared by the handlers below.</summary>
    internal static class NotificationQueryHelper
    {
        public static IQueryable<EduRate.Domain.Entities.Notification> ForUser(INotificationRepository repo, string role, int userId)
        {
            var query = repo.Query();
            return role switch
            {
                "Student" => query.Where(n => n.StudentId == userId),
                "Teacher" => query.Where(n => n.TeacherId == userId),
                _ => query.Where(n => false)
            };
        }
    }
}
