using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using EduRate.Data;
using EduRate.Models;
using System;

namespace EduRate.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        // 💡 التعديل هنا: ضفنا senderRole عشان نعرف مين بيبعت لمين
        public async Task SendMessage(int receiverId, string senderRole, string content)
        {
            var senderIdClaim = Context.User.FindFirst("ProfileId")?.Value;
            if (string.IsNullOrEmpty(senderIdClaim)) return;
            var senderId = int.Parse(senderIdClaim);

            // 💡 التعديل هنا: استخدمنا نفس أسماء الخصائص اللي في موديل الـ Message بتاعك بالظبط
            var message = new Message
            {
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false,
                SenderRole = senderRole, // بياخد "Student" أو "Teacher"
                StudentId = senderRole == "Student" ? senderId : receiverId,
                TeacherId = senderRole == "Teacher" ? senderId : receiverId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // إرسال الرسالة للطرف التاني
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, content);
        }
    }
}