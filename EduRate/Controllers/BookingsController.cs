using EduRate.Data;
using EduRate.DTOs;
using EduRate.Models;
using EduRate.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public BookingsController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // ==========================================
        // 💡 Helper Method: استخراج الـ ID من التوكن
        // ==========================================
        private int? GetUserIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
            {
                return id;
            }
            return null;
        }

        // ==========================================
        // 1. POST: إنشاء حجز جديد (خصم الفلوس من المحفظة)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Student")] // 💡 الطالب بس اللي يقدر يحجز
        public async Task<ActionResult<BookingReaddDto>> CreateBooking(BookingCreateDto dto)
        {
            var studentId = GetUserIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return BadRequest("الطالب غير موجود.");

            var session = await _context.Sessions.FindAsync(dto.SessionId);
            if (session == null) return BadRequest("الحصة غير موجودة.");

            if (session.Status == "Cancelled")
                return BadRequest("لا يمكن الحجز في حصة ملغية.");

            if (session.StartTime < DateTime.Now)
                return BadRequest("لا يمكن الحجز في حصة انتهى موعدها أو بدأت بالفعل.");

            // 💡 هندسة الأموال: التأكد من رصيد المحفظة
            if (student.WalletBalance < session.Price)
                return BadRequest("رصيد المحفظة لا يكفي لحجز هذه الحصة. يرجى الشحن أولاً.");

            // منع الحجز المزدوج لنفس الحصة
            var alreadyBooked = await _context.Bookings
                .AnyAsync(b => b.StudentId == studentId && b.SessionId == dto.SessionId && b.Status != "Cancelled");

            if (alreadyBooked)
                return BadRequest("لقد قمت بحجز هذه الحصة مسبقاً.");

            // منع تعارض مواعيد الطالب
            var hasTimeConflict = await _context.Bookings
                .Include(b => b.Session)
                .AnyAsync(b => b.StudentId == studentId &&
                               b.Status != "Cancelled" &&
                               b.Session.StartTime < session.EndTime &&
                               session.StartTime < b.Session.EndTime);

            if (hasTimeConflict)
                return BadRequest("لديك حجز آخر يتعارض مع توقيت هذه الحصة.");

            // 💡 هندسة الأموال: خصم ثمن الحصة من محفظة الطالب
            student.WalletBalance -= session.Price;

            var booking = new Booking
            {
                StudentId = (int)studentId,
                SessionId = dto.SessionId,
                BookingDate = DateTime.Now,
                Status = "Pending",
                IsAttended = false
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(); // بنحفظ الحجز وخصم الفلوس في خطوة واحدة!

            // جلب الداتا للإشعارات والـ DTO
            var createdBooking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Session).ThenInclude(s => s.Teacher)
                .Include(b => b.Session).ThenInclude(s => s.Center)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            // ==========================================
            // إرسال الإشعارات
            // ==========================================
            if (createdBooking != null)
            {
                await _notificationService.SendToStudentAsync((int)studentId, "تم تأكيد حجزك! 🎉", $"تم تأكيد حجزك بنجاح وخصم {session.Price} من محفظتك للحصة '{createdBooking.Session.Title}'.");
                await _notificationService.SendToTeacherAsync(createdBooking.Session.Teacher.Id, "حجز جديد! 📅", $"قام الطالب {createdBooking.Student.Name} بحجز مقعد في حصتك '{createdBooking.Session.Title}'.");
                await _notificationService.SendToCenterAsync(createdBooking.Session.Center.Id, "تأكيد حجز جديد 🏢", $"تم حجز مقعد جديد للطالب {createdBooking.Student.Name} في حصة المدرس {createdBooking.Session.Teacher.Name}.");
            }

            var readDto = new BookingReaddDto
            {
                Id = createdBooking.Id,
                BookingDate = createdBooking.BookingDate,
                IsAttended = createdBooking.IsAttended,
                Status = createdBooking.Status,
                SessionId = createdBooking.Session.Id,
                SessionTitle = createdBooking.Session.Title,
                SessionStartTime = createdBooking.Session.StartTime,
                SessionEndTime = createdBooking.Session.EndTime,
                SessionPrice = createdBooking.Session.Price,
                TeacherName = createdBooking.Session.Teacher.Name,
                CenterName = createdBooking.Session.Center.Name,
                StudentId = createdBooking.Student.Id,
                StudentName = createdBooking.Student.Name
            };

            return Ok(readDto);
        }

        // ==========================================
        // 2. GET: عرض حجوزات الطالب (للطالب نفسه)
        // ==========================================
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Student")] // 💡 شلنا الـ ID من الرابط، هيقرأ من التوكن
        public async Task<ActionResult<IEnumerable<BookingReaddDto>>> GetMyBookings()
        {
            var studentId = GetUserIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var bookings = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Session).ThenInclude(s => s.Teacher)
                .Include(b => b.Session).ThenInclude(s => s.Center)
                .Where(b => b.StudentId == studentId)
                .OrderByDescending(b => b.BookingDate)
                // ... (نفس لوجيك الـ Select بتاعك بالظبط)
                .Select(b => new BookingReaddDto
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    IsAttended = b.IsAttended,
                    Status = b.Status,
                    SessionId = b.Session.Id,
                    SessionTitle = b.Session.Title,
                    SessionStartTime = b.Session.StartTime,
                    SessionEndTime = b.Session.EndTime,
                    SessionPrice = b.Session.Price,
                    TeacherName = b.Session.Teacher.Name,
                    CenterName = b.Session.Center.Name,
                    StudentId = b.Student.Id,
                    StudentName = b.Student.Name
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // ==========================================
        // 3. DELETE: إلغاء الحجز (إرجاع الفلوس للطالب)
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var studentId = GetUserIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var booking = await _context.Bookings
                .Include(b => b.Session)
                .FirstOrDefaultAsync(b => b.Id == id && b.StudentId == studentId); // 💡 لازم نتأكد إن الحجز بتاعه هو!

            if (booking == null) return NotFound("الحجز غير موجود أو لا تملك صلاحية إلغائه.");

            if (booking.Status == "Cancelled")
                return BadRequest("الحجز ملغي بالفعل.");

            // 💡 حماية البيزنس: منع الإلغاء لو الحصة بدأت أو خلصت
            if (booking.Session.StartTime <= DateTime.Now)
                return BadRequest("لا يمكن إلغاء الحجز واسترداد المبلغ لأن الحصة قد بدأت أو انتهت بالفعل.");

            // 💡 هندسة الأموال: إرجاع الفلوس لمحفظة الطالب
            var student = await _context.Students.FindAsync(studentId);
            student.WalletBalance += booking.Session.Price;

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            // إشعار للطالب باسترجاع الفلوس
            await _notificationService.SendToStudentAsync((int)studentId, "تم إلغاء الحجز", $"تم إلغاء الحجز وإرجاع {booking.Session.Price} لمحفظتك.");

            return Ok(new { message = "تم إلغاء الحجز بنجاح واسترداد المبلغ." });
        }

        // ==========================================
        // 4. PATCH: تسجيل حضور الطالب
        // ==========================================
        [HttpPatch("{id}/attendance")]
        [Authorize(Roles = "Teacher,Admin")] // 💡 المدرس أو الأدمن هما اللي بياخدوا الغياب
        public async Task<IActionResult> MarkAttendance(int id, [FromBody] bool isAttended)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("الحجز غير موجود.");

            if (booking.Status == "Cancelled")
                return BadRequest("لا يمكن تسجيل حضور لحجز ملغي.");

            booking.IsAttended = isAttended;
            await _context.SaveChangesAsync();

            return Ok(new { message = isAttended ? "تم تسجيل الحضور بنجاح." : "تم إلغاء الحضور." });
        }

        // 💡 ملحوظة: دوال (GetTeacherBookings) و (GetCenterBookings) شلتهم من هنا
        // لأننا بالفعل عملناهم في الـ TeachersController والـ CentersController في الخطوة اللي فاتت تحت اسم my-bookings
    }
}