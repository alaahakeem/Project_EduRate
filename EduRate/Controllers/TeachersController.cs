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
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 💡 Helper Method: استخراج الـ ID من التوكن
        // ==========================================
        private int? GetTeacherIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
            {
                return id;
            }
            return null;
        }

        // ==========================================
        // 1. GET: عرض كل المدرسين (مفتوحة للكل)
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherReadDto>>> GetTeachers()
        {
            var teachers = await _context.Teachers
                .Include(t => t.Subject)
                .OrderByDescending(t => t.AverageRating)
                .Select(t => new TeacherReadDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    SubjectName = t.Subject != null ? t.Subject.Name : "غير محدد",
                    Bio = t.Bio,
                    YearsOfExperience = t.YearsOfExperience,
                    DemoVideoUrl = t.DemoVideoUrl,
                    TrustScore = t.TrustScore,
                    AverageRating = t.AverageRating,
                    TotalReviews = t.TotalReviews
                })
                .ToListAsync();

            return Ok(teachers);
        }

        // ==========================================
        // 2. GET: تفاصيل مدرس محدد (مفتوحة للكل)
        // ==========================================
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherReadDto>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subject)
                .Where(t => t.Id == id)
                .Select(t => new TeacherReadDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    SubjectName = t.Subject != null ? t.Subject.Name : "غير محدد",
                    Bio = t.Bio,
                    YearsOfExperience = t.YearsOfExperience,
                    DemoVideoUrl = t.DemoVideoUrl,
                    TrustScore = t.TrustScore,
                    AverageRating = t.AverageRating,
                    TotalReviews = t.TotalReviews
                })
                .FirstOrDefaultAsync();

            if (teacher == null) return NotFound(new { message = "المدرس غير موجود" });

            return Ok(teacher);
        }

        // ==========================================
        // 3. POST: إضافة مدرس جديد (للأدمن فقط لأن التسجيل من الـ Auth)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherReadDto>> PostTeacher(TeacherCreateDto dto)
        {
            var newTeacher = new Teacher
            {
                Name = dto.Name,
                SubjectId = dto.SubjectId,
                Bio = dto.Bio,
                YearsOfExperience = dto.YearsOfExperience,
                DemoVideoUrl = dto.DemoVideoUrl,
                TrustScore = 100,
                AverageRating = 0,
                TotalReviews = 0
            };

            _context.Teachers.Add(newTeacher);
            await _context.SaveChangesAsync();

            var subjectName = await _context.Subjects
                .Where(s => s.Id == dto.SubjectId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync() ?? "غير محدد";

            // ... (باقي الكود كما هو لإنشاء الـ DTO)
            return CreatedAtAction(nameof(GetTeacher), new { id = newTeacher.Id }, newTeacher);
        }

        // ==========================================
        // 4. PUT: تعديل بيانات مدرس (للمدرس نفسه من التوكن)
        // ==========================================
        [HttpPut("my-profile")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PutTeacher(TeacherUpdateDto dto)
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId == null) return Unauthorized("Invalid token.");

            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound(new { message = "المدرس غير موجود" });

            teacher.Name = dto.Name;
            teacher.SubjectId = dto.SubjectId;
            teacher.Bio = dto.Bio;
            teacher.YearsOfExperience = dto.YearsOfExperience;
            teacher.DemoVideoUrl = dto.DemoVideoUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // 5. DELETE: حذف مدرس (للأدمن فقط)
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound(new { message = "المدرس غير موجود" });

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // 6. GET: البحث والفلترة (مفتوحة)
        // ==========================================
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeacherReadDto>>> SearchTeachers([FromQuery] string? name, [FromQuery] string? subject)
        {
            var query = _context.Teachers.Include(t => t.Subject).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(t => t.Name.Contains(name));

            if (!string.IsNullOrEmpty(subject))
                query = query.Where(t => t.Subject != null && t.Subject.Name.Contains(subject));

            // ... (نفس لوجيك الـ Select)
            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ==========================================
        // 8. GET: جلب حجوزات المدرس (للمدرس نفسه)
        // ==========================================
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetTeacherBookings()
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId == null) return Unauthorized("Invalid token.");

            var bookings = await _context.Bookings
                .Where(b => b.Session.TeacherId == teacherId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingReadDto
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    StudentName = b.Student.Name
                })
                .ToListAsync();

            if (!bookings.Any()) return NotFound(new { message = "لا يوجد حجوزات حالياً" });
            return Ok(bookings);
        }

        // ==========================================
        // 9. GET: جلب السناتر وأسعار الحصص (مفتوحة للطلاب عشان يقرروا يحجزوا)
        // ==========================================
        [HttpGet("{id}/centers")]
        public async Task<ActionResult<IEnumerable<TeacherCenterReadDto>>> GetTeacherCenters(int id)
        {
            // نفس الكود بتاعك بالضبط
            var centers = await _context.TeacherCenters.Where(tc => tc.TeacherId == id).ToListAsync();
            return Ok(centers);
        }

        // ==========================================
        // 10. GET: إحصائيات لوحة التحكم (للمدرس نفسه)
        // ==========================================
        [HttpGet("my-stats")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<TeacherStatsDto>> GetTeacherStats()
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId == null) return Unauthorized("Invalid token.");

            var totalBookings = await _context.Bookings.CountAsync(b => b.Session.TeacherId == teacherId);
            var teacherStats = await _context.Teachers
                .Where(t => t.Id == teacherId)
                .Select(t => new TeacherStatsDto
                {
                    TotalBookings = totalBookings,
                    TotalReviews = t.TotalReviews,
                    AverageRating = t.AverageRating,
                    TrustScore = t.TrustScore
                })
                .FirstOrDefaultAsync();

            return Ok(teacherStats);
        }

        // ==========================================
        // 11. GET: رسائل المدرس (للمدرس نفسه)
        // ==========================================
        [HttpGet("my-messages")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<TeacherMessageDto>>> GetTeacherMessages()
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId == null) return Unauthorized("Invalid token.");

            var messages = await _context.Messages
                .Where(m => m.TeacherId == teacherId)
                .OrderByDescending(m => m.SentAt)
                // ...
                .ToListAsync();

            return Ok(messages);
        }
    }
}