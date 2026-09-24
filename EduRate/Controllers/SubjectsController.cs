using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;
using EduRate.DTOs;
using EduRate.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET: عرض كل المواد (مفتوحة للجميع)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _context.Subjects
                .Select(s => new SubjectDTOs
                {
                    Id = s.Id,
                    Name = s.Name,
                    EducationalStage = s.EducationalStage.ToString()
                })
                .ToListAsync();

            return Ok(subjects);
        }

        // ==========================================
        // 2. POST: إضافة مادة جديدة (للأدمن فقط) 🔒
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubject(SubjectCreateDto dto)
        {
            var subject = new Subject
            {
                Name = dto.Name,
                EducationalStage = Enum.Parse<EduRate.Models.EducationalStage>(dto.EducationalStage, true)
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            var subjectDto = new SubjectDTOs
            {
                Id = subject.Id,
                Name = subject.Name,
                EducationalStage = subject.EducationalStage.ToString()
            };

            return CreatedAtAction(nameof(GetAllSubjects), new { id = subject.Id }, subjectDto);
        }

        // ==========================================
        // 3. PUT: تعديل اسم المادة (للأدمن فقط) 🔒
        // ==========================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int id, SubjectCreateDto dto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound("المادة غير موجودة.");

            subject.Name = dto.Name;
            subject.EducationalStage = Enum.Parse<EduRate.Models.EducationalStage>(dto.EducationalStage, true);

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعديل المادة بنجاح." });
        }

        // ==========================================
        // 4. DELETE: حذف مادة (للأدمن فقط) 🔒
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound("المادة غير موجودة.");

            // 💡 أمان إضافي: التأكد إن المادة مفيش مدرسين مسجلين فيها قبل الحذف
            var hasTeachers = await _context.Teachers.AnyAsync(t => t.SubjectId == id);
            if (hasTeachers)
            {
                return BadRequest("لا يمكن حذف هذه المادة لوجود مدرسين مرتبطين بها.");
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المادة بنجاح." });
        }
    }
}