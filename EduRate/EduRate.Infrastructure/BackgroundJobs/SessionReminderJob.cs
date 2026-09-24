using EduRate.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// THE ONE Hangfire use case in this project. Replaces the original SessionReminderService
    /// (a naive BackgroundService with its own Task.Delay(15 min) polling loop) with a Hangfire
    /// recurring job: same "notify teacher + booked students ~2h before session start" logic,
    /// but scheduled/retried/observed through Hangfire instead of a hand-rolled loop.
    ///
    /// Registered as a recurring job (every 15 minutes, matching the original polling interval)
    /// in Infrastructure.DependencyInjection / API Program.cs via RecurringJob.AddOrUpdate.
    /// </summary>
    public class SessionReminderJob
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationService _notificationService;

        public SessionReminderJob(
            ISessionRepository sessionRepository,
            IBookingRepository bookingRepository,
            INotificationService notificationService)
        {
            _sessionRepository = sessionRepository;
            _bookingRepository = bookingRepository;
            _notificationService = notificationService;
        }

        public async Task CheckUpcomingSessionsAsync()
        {
            var now = DateTime.Now;

            // Sessions starting in ~2 hours (a 15-minute window, matching the job's own cadence).
            var windowStart = now.AddHours(1).AddMinutes(45);
            var windowEnd = now.AddHours(2);

            var upcomingSessions = await _sessionRepository.Query()
                .Include(s => s.Teacher)
                .Where(s => s.StartTime > windowStart && s.StartTime <= windowEnd && s.Status != "Cancelled")
                .ToListAsync();

            foreach (var session in upcomingSessions)
            {
                await _notificationService.SendToTeacherAsync(
                    session.TeacherId,
                    "تذكير بموعد الحصة \u23f0",
                    $"حصتك '{session.Title}' ستبدأ قريباً خلال ساعتين."
                );

                var bookings = await _bookingRepository.Query()
                    .Include(b => b.Student)
                    .Where(b => b.SessionId == session.Id && b.Status != "Cancelled")
                    .ToListAsync();

                foreach (var booking in bookings)
                {
                    await _notificationService.SendToStudentAsync(
                        booking.StudentId,
                        "استعد للحصة! \ud83d\ude80",
                        $"حصتك '{session.Title}' مع المدرس {session.Teacher.Name} ستبدأ خلال ساعتين. بالتوفيق!"
                    );
                }
            }
        }
    }
}
