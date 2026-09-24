using EduRate.Data;
using EduRate.DTOs;
using EduRate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymobService _paymobService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymobService paymobService, AppDbContext context, IConfiguration configuration)
        {
            _paymobService = paymobService;
            _context = context;
            _configuration = configuration;
        }

        // ==========================================
        // 💡 Helper Method: استخراج الـ ID من التوكن
        // ==========================================
        private int? GetStudentIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
                return id;
            return null;
        }

        // ==========================================
        // 1. POST: طلب شحن المحفظة (بيرجع رابط الدفع)
        // ==========================================
        [HttpPost("charge")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ChargeWallet([FromBody] PaymentRequestDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("الطالب غير موجود.");

            if (dto.Amount <= 0) return BadRequest("المبلغ يجب أن يكون أكبر من الصفر.");

            // 1. جلب توكن المصادقة من Paymob
            var authToken = await _paymobService.GetAuthTokenAsync();

            // 2. تسجيل الطلب (Order)
            var orderId = await _paymobService.CreateOrderAsync(authToken, dto.Amount);

            // 3. طلب مفتاح الدفع (Payment Key)
            var paymentKey = await _paymobService.GetPaymentKeyAsync(
                authToken,
                orderId,
                dto.Amount,
                dto.Email ?? student.Email,
                dto.FirstName ?? student.Name,
                dto.LastName ?? "Student",
                dto.PhoneNumber,
                dto.PaymentMethod // 💡 التعديل هنا: تمرير طريقة الدفع للخدمة (Card أو Wallet)
            );

            // 4. تجهيز رابط الدفع (Iframe)
            var iframeId = _configuration["Paymob:IframeId"]; // رقم شاشة الدفع اللي بتتكريت في لوحة تحكم Paymob
            var iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

            return Ok(new PaymentResponseDto
            {
                RedirectUrl = iframeUrl,
                Message = "تم تجهيز رابط الدفع بنجاح. يرجى التوجه للرابط لإتمام العملية."
            });
        }

        // ==========================================
        // 2. POST: Webhook (الرد الآلي من Paymob) 🔒
        // ==========================================
        [HttpPost("callback")]
        [AllowAnonymous] // 💡 لازم تكون مفتوحة عشان سيرفر Paymob يقدر يكلمها
        public async Task<IActionResult> PaymobWebhook([FromBody] JsonElement payload)
        {
            // Paymob بيبعت داتا كتير، إحنا يهمنا نعرف العملية نجحت ولا لأ
            var obj = payload.GetProperty("obj");
            var success = obj.GetProperty("success").GetBoolean();

            // لو العملية فشلت، مش هنعمل حاجة
            if (!success) return Ok();

            // لو نجحت، هنجيب بيانات الطالب والمبلغ
            var amountCents = obj.GetProperty("amount_cents").GetInt32();
            var amountEgp = amountCents / 100m; // بنحول القروش لجنيهات

            // في البيزنس الحقيقي، بنكون رابطين الـ order_id برقم الطالب في جدول (Transactions)،
            // بس للتبسيط هنا، هنستخرج الإيميل اللي اتبعت في الـ billing_data عشان نعرف مين الطالب اللي دفع
            var billingData = obj.GetProperty("order").GetProperty("billing_data");
            var studentEmail = billingData.GetProperty("email").GetString();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
            if (student != null)
            {
                // 💡 هندسة الأموال: تحديث المحفظة أخيراً!
                student.WalletBalance += amountEgp;
                await _context.SaveChangesAsync();
            }

            // السيرفر لازم يرد بـ 200 OK عشان Paymob يعرف إننا استلمنا الرسالة
            return Ok();
        }
    }
}