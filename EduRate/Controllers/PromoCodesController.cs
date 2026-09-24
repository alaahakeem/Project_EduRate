using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;
using EduRate.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")] // 💡 قفلنا البوابة دي للطلاب بس
    public class PromoCodesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromoCodesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePromoCode([FromBody] ValidatePromoCodeDto dto)
        {
            var promo = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code == dto.Code && p.IsActive);

            if (promo == null)
                return BadRequest("كود الخصم غير صحيح أو غير مفعل.");

            if (promo.ExpiryDate < System.DateTime.Now)
                return BadRequest("انتهت صلاحية كود الخصم.");

            if (promo.CurrentUsageCount >= promo.MaxUsageCount)
                return BadRequest("تم الوصول للحد الأقصى لاستخدام هذا الكود.");

            // 💡 هندسة اللوجيك: لو الكود مخصص لمدرس معين، نتأكد إن الطالب رايح يحجز للمدرس ده!
            // (بافتراض إن جدول PromoCodes عندك فيه TeacherId و CenterId)
            /* 
            if (promo.TeacherId != null && promo.TeacherId != dto.TeacherId)
                return BadRequest("كود الخصم هذا غير صالح لهذا المدرس.");

            if (promo.CenterId != null && promo.CenterId != dto.CenterId)
                return BadRequest("كود الخصم هذا غير صالح لهذا السنتر.");
            */

            return Ok(new
            {
                Message = "كود الخصم صالح!",
                DiscountPercentage = promo.DiscountPercentage
            });
        }
    }
}