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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public CentersController(AppDbContext context, IWebHostEnvironment env, INotificationService notificationService)
        {
            _context = context;
            _env = env;
            _notificationService = notificationService;
        }

        // 💡 بنجيب رقم الطالب من التوكن عشان التقييمات
        // وهنستخدمه كمان للسنتر عشان نجيب بروفايله الخاص
        private int? GetProfileIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
                return id;
            return null;
        }

        #region 💡 لوحة تحكم السنتر (Endpoints جديدة خاصة بالسنتر)

        [HttpGet("my-profile")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> GetMyProfile()
        {
            var centerId = GetProfileIdFromToken();
            if (centerId == null) return Unauthorized();

            var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == centerId);
            if (center == null) return NotFound("لم يتم العثور على بيانات السنتر.");

            return Ok(center);
        }

        [HttpPut("my-profile")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] CenterUpdateDto dto)
        {
            var centerId = GetProfileIdFromToken();
            if (centerId == null) return Unauthorized();

            var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == centerId);
            if (center == null) return NotFound();

            center.Name = dto.Name ?? center.Name;
            center.Address = dto.Location ?? center.Address;
            center.Latitude = dto.Latitude ?? center.Latitude;
            center.Longitude = dto.Longitude ?? center.Longitude;

            await _context.SaveChangesAsync();
            return Ok(center);
        }

        [HttpGet("my-sessions")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> GetMySessions()
        {
            var centerId = GetProfileIdFromToken();
            if (centerId == null) return Unauthorized();

            // بيفترض إن عندك جدول Sessions مربوط بـ CenterId
            var sessions = await _context.Sessions
                .Where(s => s.CenterId == centerId)
                .Include(s => s.Teacher)
                .ToListAsync();

            return Ok(sessions);
        }

        #endregion

        #region 1. CRUD (الأساسيات)

        // GET: api/centers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CenterDto>>> GetCenters([FromQuery] bool onlyVerified = false)
        {
            // (نفس اللوجيك بتاعك للبحث والعرض بدون تعديل)
            var query = _context.Centers.AsQueryable();
            if (onlyVerified) query = query.Where(c => c.IsVerified);
            var centers = await query.Select(c => new CenterDto { Id = c.Id, Name = c.Name, Location = c.Address, IsVerified = c.IsVerified }).ToListAsync();
            return Ok(centers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CenterDetailsDto>> GetCenter(int id)
        {
            var center = await _context.Centers.Select(c => new CenterDetailsDto { Id = c.Id, Name = c.Name, Location = c.Address, Latitude = c.Latitude, Longitude = c.Longitude, IsVerified = c.IsVerified }).FirstOrDefaultAsync(c => c.Id == id);
            if (center == null) return NotFound("Center not found.");
            return Ok(center);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // 💡 الأدمن بس يكريت سنتر
        public async Task<ActionResult<CenterDetailsDto>> CreateCenter(CenterCreateDto dto)
        {
            var center = new Center { Name = dto.Name, Address = dto.Location, Latitude = dto.Latitude, Longitude = dto.Longitude, IsVerified = false };
            _context.Centers.Add(center);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCenter), new { id = center.Id }, center);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // 💡 التعديل للأدمن
        public async Task<IActionResult> UpdateCenter(int id, CenterUpdateDto dto)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center == null) return NotFound("Center not found.");
            center.Name = dto.Name; center.Address = dto.Location; center.Latitude = dto.Latitude; center.Longitude = dto.Longitude;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // 💡 الحذف للأدمن
        public async Task<IActionResult> DeleteCenter(int id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center == null) return NotFound("Center not found.");
            _context.Centers.Remove(center);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region 2. البحث والفلترة (تجربة الطالب)
        // (الدوال دي Search و Nearby و Top كلها زي ما هي مفتوحة للكل)
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CenterDto>>> SearchCenters([FromQuery] string name, [FromQuery] string location) { /* ... */ return Ok(); }

        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<CenterDto>>> GetNearbyCenters([FromQuery] double lat, [FromQuery] double lng) { /* ... */ return Ok(); }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<CenterDto>>> GetTopCenters() { /* ... */ return Ok(); }
        #endregion

        #region 3. الشغل التقيل والإدارة (ربط المدرسين)
        [HttpGet("{id}/teachers")]
        public async Task<ActionResult<IEnumerable<CenterTeacherDto>>> GetCenterTeachers(int id) { /* ... */ return Ok(); }

        [HttpPost("{id}/teachers")]
        [Authorize(Roles = "Admin")] // 💡 الأدمن هو اللي بيربط مدرس بسنتر
        public async Task<IActionResult> AddTeacherToCenter(int id, AddTeacherToCenterDto dto)
        {
            /* ... (اللوجيك بتاعك زي ما هو) ... */
            return Ok("Teacher added to center successfully.");
        }

        [HttpPut("{id}/teachers/{teacherId}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleTeacherStatus(int id, int teacherId) { /* ... */ return Ok(); }
        #endregion

        #region 4. التقييمات، المواعيد، الإحصائيات والتوثيق

        [HttpPost("{id}/reviews")]
        [Authorize(Roles = "Student")] // 💡 الطالب بس اللي يقيم
        public async Task<IActionResult> AddCenterReview(int id, CenterReviewCreateDto dto)
        {
            var studentId = GetProfileIdFromToken(); // 💡 استخدمنا الهيلبر بعد تعديل اسمه
            if (studentId == null) return Unauthorized("Invalid token.");
            if (!await _context.Centers.AnyAsync(c => c.Id == id)) return NotFound("Center not found.");

            var review = new Review
            {
                CenterId = id,
                StudentId = (int)studentId, // 💡 خدنا الـ ID من التوكن مش من الـ DTO عشان الأمان!
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return Ok("Review added successfully.");
        }

        [HttpGet("{id}/schedule")]
        public async Task<ActionResult<IEnumerable<CenterScheduleDto>>> GetCenterSchedule(int id) { /* ... */ return Ok(); }

        [HttpPatch("{id}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyCenter(int id) { /* ... */ return NoContent(); }

        [HttpGet("{id}/stats")]
        [Authorize(Roles = "Admin")] // يفضل الإحصائيات للأدمن
        public async Task<ActionResult<CenterStatsDto>> GetCenterStats(int id) { /* ... */ return Ok(); }
        #endregion

        #region 5. صور السنتر (Gallery)
        [HttpGet("{id}/images")]
        public async Task<ActionResult<IEnumerable<CenterImageDto>>> GetCenterImages(int id) { /* ... */ return Ok(); }

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCenterImage(int id, IFormFile file, [FromQuery] bool isMain = false) { /* ... */ return Ok(); }
        #endregion
    }
}