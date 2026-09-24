using EduRate.Infrastructure.Persistence;
using EduRate.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EduRate.Infrastructure.Realtime
{
    /// <summary>
    /// Kept in Infrastructure (rather than the API project) because NotificationService -
    /// also Infrastructure - needs IHubContext&lt;ChatHub&gt; to push live notifications, and
    /// Infrastructure cannot depend on the API project. The API project maps this hub's
    /// route in Program.cs (it already references Infrastructure).
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int receiverId, string senderRole, string content)
        {
            var senderIdClaim = Context.User?.FindFirst("ProfileId")?.Value;
            if (string.IsNullOrEmpty(senderIdClaim)) return;
            var senderId = int.Parse(senderIdClaim);

            var message = new Message
            {
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false,
                SenderRole = senderRole, // "Student" or "Teacher"
                StudentId = senderRole == "Student" ? senderId : receiverId,
                TeacherId = senderRole == "Teacher" ? senderId : receiverId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, content);
        }
    }
}
