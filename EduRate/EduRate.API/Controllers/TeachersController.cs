using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Teachers.Commands;
using EduRate.Application.Features.Teachers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TeachersController(IMediator mediator) => _mediator = mediator;

        private int GetTeacherId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. List (open to everyone)
        [HttpGet]
        public async Task<IActionResult> GetTeachers()
            => Ok(await _mediator.Send(new GetTeachersQuery()));

        // 2. Details (open to everyone)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacher(int id)
            => Ok(await _mediator.Send(new GetTeacherQuery { Id = id }));

        // 3. Create (Admin only - registration itself goes through Auth)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostTeacher(CreateTeacherCommand command)
        {
            var newTeacher = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTeacher), new { id = newTeacher.Id }, newTeacher);
        }

        // 4. Update own profile (Teacher only)
        [HttpPut("my-profile")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PutTeacher(UpdateTeacherCommand command)
        {
            command.TeacherId = GetTeacherId();
            await _mediator.Send(command);
            return NoContent();
        }

        // 5. Delete (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _mediator.Send(new DeleteTeacherCommand { TeacherId = id });
            return NoContent();
        }

        // 6. Search (open)
        [HttpGet("search")]
        public async Task<IActionResult> SearchTeachers([FromQuery] string? name, [FromQuery] string? subject)
            => Ok(await _mediator.Send(new SearchTeachersQuery { Name = name, Subject = subject }));

        // 8. Own bookings (Teacher only)
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherBookings()
            => Ok(await _mediator.Send(new GetTeacherBookingsQuery { TeacherId = GetTeacherId() }));

        // 9. Centers a teacher works at (open)
        [HttpGet("{id}/centers")]
        public async Task<IActionResult> GetTeacherCenters(int id)
            => Ok(await _mediator.Send(new GetTeacherCentersQuery { TeacherId = id }));

        // 10. Dashboard stats (Teacher only)
        [HttpGet("my-stats")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherStats()
            => Ok(await _mediator.Send(new GetTeacherStatsQuery { TeacherId = GetTeacherId() }));

        // 11. Own messages (Teacher only)
        [HttpGet("my-messages")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherMessages()
            => Ok(await _mediator.Send(new GetTeacherMessagesQuery { TeacherId = GetTeacherId() }));
    }
}
