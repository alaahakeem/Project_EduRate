using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Messages.Commands;
using EduRate.Application.Features.Messages.Queries;
using EduRate.Infrastructure.Realtime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,Teacher")]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(IMediator mediator, IHubContext<ChatHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(profileIdClaim) || !int.TryParse(profileIdClaim, out int senderId) || string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedException("Invalid token.");

            command.SenderId = senderId;
            command.SenderRole = roleClaim;

            var result = await _mediator.Send(command);

            // Live push over SignalR - same "SendAsync to the receiver" the original controller did.
            await _hubContext.Clients.User(command.ReceiverId.ToString())
                             .SendAsync("ReceiveMessage", senderId, command.Content);

            return Ok(new { message = result });
        }

        [HttpGet("conversation/{receiverId}")]
        public async Task<IActionResult> GetConversation(int receiverId)
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(profileIdClaim) || !int.TryParse(profileIdClaim, out int myId))
                throw new UnauthorizedException("Invalid token.");

            var query = new GetConversationQuery { MyId = myId, MyRole = roleClaim ?? string.Empty, ReceiverId = receiverId };
            return Ok(await _mediator.Send(query));
        }
    }
}
