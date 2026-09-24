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
    public class SessionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public SessionsController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // ==========================================
        // 💡 Helper Method: سحب رقم المدرس من التوكن
        // ==========================================
        private int? GetTeacherIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
                return id;
            return null;
        }

        // ==========================================
        // 1. GET: عرض كل الحصص (مفتوحة للجميع)
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionReadDto>>> GetSessions()
        {
            var sessions = await _context.Sessions
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .Where(s => s.Status == "Available") // 💡 تعديل بسيط: نعرض المتاح بس للناس
                .Select(s => new SessionReadDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Price = s.Price,
                    EducationalStage = s.EducationalStage,
                    Status = s.Status,
                    CenterName = s.Center.Name,
                    TeacherName = s.Teacher.Name
                })
                .ToListAsync();

            return Ok(sessions);
        }

        // ==========================================
        // 2. GET: عرض الحصص الخاصة بمرحلة دراسية (مفتوحة للجميع)
        // ==========================================
        [HttpGet("stage/{stage}")]
        public async Task<ActionResult<IEnumerable<SessionReadDto>>> GetSessionsByStage(string stage)
        {
            var sessions = await _context.Sessions
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .Where(s => s.EducationalStage == stage && s.Status == "Available")
                .OrderBy(s => s.StartTime)
                .Select(s => new SessionReadDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Price = s.Price,
                    EducationalStage = s.EducationalStage,
                    Status = s.Status,
                    CenterName = s.Center.Name,
                    TeacherName = s.Teacher.Name
                })
                .ToListAsync();

            return Ok(sessions);
        }

        // ==========================================
        // 3. POST: إنشاء حصة جديدة
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Teacher")] // 💡 المدرس بس هو اللي يكريت حصة
        public async Task<ActionResult<SessionReadDto>> CreateSession(SessionCreateDto dto)
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId == null) return Unauthorized("Invalid token.");

            if (dto.StartTime < DateTime.Now)
                return BadRequest("لا يمكن إنشاء حصة في موعد قديم.");

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("موعد انتهاء الحصة يجب أن يكون بعد موعد بدايتها.");

            var centerExists = await _context.Centers.AnyAsync(c => c.Id == dto.CenterId);
            if (!centerExists)
                return BadRequest("السنتر غير موجود في قاعدة البيانات.");

            var hasConflict = await _context.Sessions
                .AnyAsync(s => s.TeacherId == teacherId && // 💡 بنقارن بـ teacherId بتاع التوكن
                               s.Status != "Cancelled" &&
                               s.StartTime < dto.EndTime &&
                               dto.StartTime < s.EndTime);

            if (hasConflict)
                return BadRequest("لديك حصة أخرى في نفس هذا التوقيت.");

            var newSession = new Session
            {
                Title = dto.Title,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Price = dto.Price,
                EducationalStage = dto.EducationalStage,
                CenterId = dto.CenterId,
                TeacherId = (int)teacherId, // 💡 جبناه من التوكن مباشرة
                Status = "Available"
            };

            _context.Sessions.Add(newSession);
            await _context.SaveChangesAsync();

            var createdSession = await _context.Sessions
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .FirstOrDefaultAsync(s => s.Id == newSession.Id);

            if (createdSession != null)
            {
                await _notificationService.SendToCenterAsync(
                    createdSession.CenterId,
                    "حصة جديدة مجدولة 📅",
                    $"قام المدرس {createdSession.Teacher.Name} بجدولة حصة '{createdSession.Title}' في السنتر الخاص بك يوم {createdSession.StartTime:yyyy-MM-dd}."
                );
            }

            var readDto = new SessionReadDto
            {
                Id = createdSession.Id,
                Title = createdSession.Title,
                StartTime = createdSession.StartTime,
                EndTime = createdSession.EndTime,
                Price = createdSession.Price,
                EducationalStage = createdSession.EducationalStage,
                Status = createdSession.Status,
                CenterName = createdSession.Center.Name,
                TeacherName = createdSession.Teacher.Name
            };

            return CreatedAtAction(nameof(GetSessions), new { id = newSession.Id }, readDto);
        }

        // ==========================================
        // 4. GET: عرض الطلبة اللي حجزوا الحصة
        // ==========================================
        [HttpGet("{id}/bookings")]
        [Authorize(Roles = "Teacher,Admin")] // 💡 حماية للكشف
        public async Task<ActionResult> GetSessionBookings(int id)
        {
            var teacherId = GetTeacherIdFromToken();
            var session = await _context.Sessions.FindAsync(id);

            if (session == null) return NotFound("الحصة غير موجودة");

            // 💡 التأكد إن المدرس ده هو صاحب الحصة
            if (User.IsInRole("Teacher") && session.TeacherId != teacherId)
                return Forbid("لا تملك صلاحية عرض كشف هذه الحصة.");

            var bookings = await _context.Bookings
                .Include(b => b.Student)
                .Where(b => b.SessionId == id)
                .Select(b => new
                {
                    BookingId = b.Id,
                    StudentName = b.Student.Name,
                    BookingDate = b.BookingDate,
                    Status = b.Status
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // ==========================================
        // 5. PUT: تعديل بيانات الحصة
        // ==========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateSession(int id, SessionUpdateDto dto)
        {
            var teacherId = GetTeacherIdFromToken();
            var session = await _context.Sessions.FindAsync(id);

            if (session == null) return NotFound("الحصة غير موجودة.");

            // 💡 التأكد من الصلاحية
            if (session.TeacherId != teacherId)
                return Forbid("لا تملك صلاحية تعديل هذه الحصة.");

            if (dto.StartTime < DateTime.Now)
                return BadRequest("لا يمكن تعديل الحصة لموعد قديم.");
            if (dto.EndTime <= dto.StartTime)
                return BadRequest("موعد انتهاء الحصة يجب أن يكون بعد موعد بدايتها.");

            var hasConflict = await _context.Sessions
                .AnyAsync(s => s.TeacherId == session.TeacherId &&
                               s.Id != id &&
                               s.Status != "Cancelled" &&
                               s.StartTime < dto.EndTime &&
                               dto.StartTime < s.EndTime);

            if (hasConflict)
                return BadRequest("لديك حصة أخرى في التوقيت الجديد.");

            session.Title = dto.Title;
            session.StartTime = dto.StartTime;
            session.EndTime = dto.EndTime;
            session.Price = dto.Price;
            session.EducationalStage = dto.EducationalStage;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تعديل الحصة بنجاح." });
        }

        // ==========================================
        // 6. DELETE: إلغاء الحصة واسترداد الأموال 💸
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CancelSession(int id)
        {
            var teacherId = GetTeacherIdFromToken();
            var session = await _context.Sessions
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.Student) // 💡 بنجيب بيانات الطلاب المرتبطين عشان نرجع الفلوس
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound("الحصة غير موجودة.");

            // 💡 التأكد من الصلاحية
            if (session.TeacherId != teacherId)
                return Forbid("لا تملك صلاحية إلغاء هذه الحصة.");

            if (session.Status == "Cancelled")
                return BadRequest("هذه الحصة ملغية بالفعل.");

            session.Status = "Cancelled";

            // 💡 هندسة الأموال: إلغاء كل الحجوزات ورد المبالغ
            if (session.Bookings != null)
            {
                foreach (var booking in session.Bookings.Where(b => b.Status != "Cancelled"))
                {
                    booking.Status = "Cancelled";

                    if (booking.Student != null)
                    {
                        booking.Student.WalletBalance += session.Price; // إرجاع الفلوس

                        await _notificationService.SendToStudentAsync(
                            booking.Student.Id,
                            "إلغاء حصة واسترداد أموال ⚠️",
                            $"نعتذر، تم إلغاء حصة '{session.Title}'. تمت إعادة مبلغ {session.Price} إلى محفظتك."
                        );
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم إلغاء الحصة، واسترداد الأموال للطلاب في محافظهم بنجاح." });
        }
    }
}