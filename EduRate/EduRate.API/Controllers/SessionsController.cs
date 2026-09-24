using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Sessions.Commands;
using EduRate.Application.Features.Sessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SessionsController(IMediator mediator) => _mediator = mediator;

        private int GetTeacherId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. All available sessions (open)
        [HttpGet]
        public async Task<IActionResult> GetSessions()
            => Ok(await _mediator.Send(new GetSessionsQuery()));

        // 2. By educational stage (open)
        [HttpGet("stage/{stage}")]
        public async Task<IActionResult> GetSessionsByStage(string stage)
            => Ok(await _mediator.Send(new GetSessionsByStageQuery { Stage = stage }));

        // 3. Create (Teacher only)
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateSession(CreateSessionCommand command)
        {
            command.TeacherId = GetTeacherId();
            var readDto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSessions), new { id = readDto.Id }, readDto);
        }

        // 4. Roster for a session (Teacher or Admin)
        [HttpGet("{id}/bookings")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GetSessionBookings(int id)
            => Ok(await _mediator.Send(new GetSessionBookingsQuery { SessionId = id }));

        // 5. Update (Teacher only, must own the session)
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateSession(int id, UpdateSessionCommand command)
        {
            command.SessionId = id;
            var message = await _mediator.Send(command);
            return Ok(new { message });
        }

        // 6. Cancel + refund (Teacher only, must own the session)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CancelSession(int id)
        {
            var message = await _mediator.Send(new CancelSessionCommand { SessionId = id });
            return Ok(new { message });
        }
    }
}
