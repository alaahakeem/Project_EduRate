using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Queries
{
    /// <summary>Student-scoped notifications list, as originally exposed under StudentController's "my-notifications".</summary>
    public class GetMyNotificationsQuery : IRequest<List<StudentNotificationDto>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<StudentNotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        public GetMyNotificationsQueryHandler(INotificationRepository notificationRepository) => _notificationRepository = notificationRepository;

        public async Task<List<StudentNotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await _notificationRepository.Query()
                .Where(n => n.StudentId == request.StudentId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new StudentNotificationDto
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
}
