using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;
using EduRate.DTOs;
using EduRate.Models;
using EduRate.Hubs; // 💡 ضفنا مسار السنترال
using Microsoft.AspNetCore.SignalR; // 💡 ضفنا مكتبة SignalR
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,Teacher")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext; // 💡 عرفنا السنترال هنا

        public MessagesController(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext; // 💡 حقناه في الـ Constructor
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            // 1. استخراج بيانات المرسل من التوكن أوتوماتيك
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(profileIdClaim) || !int.TryParse(profileIdClaim, out int senderId) || string.IsNullOrEmpty(roleClaim))
            {
                return Unauthorized("Invalid token.");
            }

            // 2. تجهيز الرسالة
            var message = new Message
            {
                Content = dto.Content,
                SentAt = System.DateTime.Now,
                IsRead = false,
                SenderRole = roleClaim
            };

            // 3. تحديد مين بيبعت لمين
            if (roleClaim == "Student")
            {
                message.StudentId = senderId;
                message.TeacherId = dto.ReceiverId;
            }
            else if (roleClaim == "Teacher")
            {
                message.TeacherId = senderId;
                message.StudentId = dto.ReceiverId;
            }

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // 💡 4. السحر: إرسال الرسالة "لايف" للطرف التاني عبر SignalR
            await _hubContext.Clients.User(dto.ReceiverId.ToString())
                             .SendAsync("ReceiveMessage", senderId, dto.Content);

            return Ok(new { message = "Message sent successfully." });
        }

        [HttpGet("conversation/{receiverId}")]
        public async Task<IActionResult> GetConversation(int receiverId)
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(profileIdClaim) || !int.TryParse(profileIdClaim, out int myId))
            {
                return Unauthorized("Invalid token.");
            }

            int searchStudentId = roleClaim == "Student" ? myId : receiverId;
            int searchTeacherId = roleClaim == "Teacher" ? myId : receiverId;

            var messages = await _context.Messages
                .Where(m => m.StudentId == searchStudentId && m.TeacherId == searchTeacherId)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderRole = m.SenderRole,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}