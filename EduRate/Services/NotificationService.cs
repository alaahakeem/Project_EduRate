using EduRate.Data;
using EduRate.Models;
using EduRate.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace EduRate.Services
{ 

    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationService(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
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
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

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
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

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
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(centerId.ToString())
                             .SendAsync("ReceiveNotification", title, message);
        }
    }
}