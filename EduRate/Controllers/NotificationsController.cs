using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;
using EduRate.Models;
using EduRate.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 💡 قفلنا الكنترولر على أي حد مسجل دخول (طالب، مدرس، أدمن)
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 💡 1. Helper Method: سحب بيانات اليوزر ونوعه من التوكن
        // ==========================================
        private (int UserId, string Role)? GetUserDetailsFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int userId) && !string.IsNullOrEmpty(roleClaim))
            {
                return (userId, roleClaim);
            }
            return null;
        }

        // ==========================================
        // 💡 2. Helper Method: الفلترة حسب نوع اليوزر
        // ==========================================
        private IQueryable<Notification> GetUserNotificationsQuery(string role, int userId)
        {
            var query = _context.Notifications.AsQueryable();

            return role switch
            {
                "Student" => query.Where(n => n.StudentId == userId),
                "Teacher" => query.Where(n => n.TeacherId == userId),
                // لو عاملة Role للسنتر ضيفيه هنا، مثلاً "Center"
                _ => query.Where(n => false) // يرجع فاضي لو الرول مش معروف
            };
        }

        // ==========================================
        // 3. GET: جلب أحدث 50 إشعار للمستخدم (من التوكن)
        // ==========================================
        [HttpGet("my-notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications()
        {
            var userDetails = GetUserDetailsFromToken();
            if (userDetails == null) return Unauthorized("Invalid token.");

            var query = GetUserNotificationsQuery(userDetails.Value.Role, userDetails.Value.UserId);

            var notifications = await query
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
                .ToListAsync();

            return Ok(notifications);
        }

        // ==========================================
        // 4. GET: جلب عدد الإشعارات غير المقروءة 
        // ==========================================
        [HttpGet("my-unread-count")]
        public async Task<ActionResult> GetMyUnreadCount()
        {
            var userDetails = GetUserDetailsFromToken();
            if (userDetails == null) return Unauthorized("Invalid token.");

            var query = GetUserNotificationsQuery(userDetails.Value.Role, userDetails.Value.UserId);
            var count = await query.CountAsync(n => !n.IsRead);

            return Ok(new { unreadCount = count });
        }

        // ==========================================
        // 5. PUT: تحديد إشعار معين كمقروء
        // ==========================================
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userDetails = GetUserDetailsFromToken();
            if (userDetails == null) return Unauthorized("Invalid token.");

            // 💡 الأمان هنا: بنتأكد إن الإشعار ده فعلاً يخص اليوزر اللي بيحاول يخليه مقروء!
            var query = GetUserNotificationsQuery(userDetails.Value.Role, userDetails.Value.UserId);
            var notification = await query.FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null) return NotFound(new { message = "الإشعار غير موجود أو لا تملك صلاحية تعديله." });

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديد الإشعار كمقروء" });
        }

        // ==========================================
        // 6. PUT: تحديد كل إشعارات المستخدم كمقروءة
        // ==========================================
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userDetails = GetUserDetailsFromToken();
            if (userDetails == null) return Unauthorized("Invalid token.");

            var query = GetUserNotificationsQuery(userDetails.Value.Role, userDetails.Value.UserId);
            var unreadNotifications = await query.Where(n => !n.IsRead).ToListAsync();

            if (!unreadNotifications.Any()) return Ok(new { message = "لا يوجد إشعارات جديدة" });

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديد جميع الإشعارات كمقروءة" });
        }
    }
}