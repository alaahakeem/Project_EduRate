using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;

namespace EduRate.Services
{
    public class SessionReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SessionReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // اللوب دي هتفضل شغالة طول ما السيرفر شغال
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckUpcomingSessionsAsync();

                // هنخلي المنبه ينام 15 دقيقة، وبعدين يصحى يعيد اللفة تاني
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private async Task CheckUpcomingSessionsAsync()
        {
            // 💡 بنعمل Scope مؤقت عشان نقدر نستخدم الـ DbContext و الـ NotificationService
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var now = DateTime.Now;

            // هنشوف الحصص اللي هتبدأ كمان ساعتين (بندي لنفسنا مساحة 15 دقيقة عشان المنبه بيلف كل 15 دقيقة)
            var windowStart = now.AddHours(1).AddMinutes(45);
            var windowEnd = now.AddHours(2);

            // نجيب الحصص اللي هتبدأ في الوقت ده ومش ملغية
            var upcomingSessions = await context.Sessions
                .Include(s => s.Teacher)
                .Where(s => s.StartTime > windowStart && s.StartTime <= windowEnd && s.Status != "Cancelled")
                .ToListAsync();

            foreach (var session in upcomingSessions)
            {
                // 1. إرسال تذكير للمدرس
                await notificationService.SendToTeacherAsync(
                    session.TeacherId,
                    "تذكير بموعد الحصة ⏰",
                    $"حصتك '{session.Title}' ستبدأ قريباً خلال ساعتين."
                );

                // 2. نجيب الطلاب اللي حاجزين الحصة دي عشان نبعتلهم تذكير
                var bookings = await context.Bookings
                    .Include(b => b.Student)
                    .Where(b => b.SessionId == session.Id && b.Status != "Cancelled")
                    .ToListAsync();

                foreach (var booking in bookings)
                {
                    await notificationService.SendToStudentAsync(
                        booking.StudentId,
                        "استعد للحصة! 🚀",
                        $"حصتك '{session.Title}' مع المدرس {session.Teacher.Name} ستبدأ خلال ساعتين. بالتوفيق!"
                    );
                }
            }
        }
    }
}