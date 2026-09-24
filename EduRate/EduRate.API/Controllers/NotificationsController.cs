using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Notifications.Commands;
using EduRate.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any authenticated user - Student, Teacher, or Admin
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationsController(IMediator mediator) => _mediator = mediator;

        private (int UserId, string Role) GetUserDetails()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int userId) && !string.IsNullOrEmpty(roleClaim))
                return (userId, roleClaim);

            throw new UnauthorizedException("Invalid token.");
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var (userId, role) = GetUserDetails();
            return Ok(await _mediator.Send(new GetMyNotificationsQuery { UserId = userId, Role = role }));
        }

        [HttpGet("my-unread-count")]
        public async Task<IActionResult> GetMyUnreadCount()
        {
            var (userId, role) = GetUserDetails();
            var count = await _mediator.Send(new GetMyUnreadCountQuery { UserId = userId, Role = role });
            return Ok(new { unreadCount = count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var (userId, role) = GetUserDetails();
            var message = await _mediator.Send(new MarkAsReadCommand { NotificationId = id, UserId = userId, Role = role });
            return Ok(new { message });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var (userId, role) = GetUserDetails();
            var message = await _mediator.Send(new MarkAllAsReadCommand { UserId = userId, Role = role });
            return Ok(new { message });
        }
    }
}
