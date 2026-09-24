using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using EduRate.Infrastructure.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace EduRate.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task SendToStudentAsync(int studentId, string title, string message)
        {
            var notification = new Notification
            {
                StudentId = studentId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _notificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            await _hubContext.Clients.User(studentId.ToString())
                             .SendAsync("ReceiveNotification", title, message);
        }

        public async Task SendToTeacherAsync(int teacherId, string title, string message)
        {
            var notification = new Notification
            {
                TeacherId = teacherId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _notificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            await _hubContext.Clients.User(teacherId.ToString())
                             .SendAsync("ReceiveNotification", title, message);
        }

        public async Task SendToCenterAsync(int centerId, string title, string message)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _notificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            await _hubContext.Clients.User(centerId.ToString())
                             .SendAsync("ReceiveNotification", title, message);
        }
    }
}
