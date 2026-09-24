using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Bookings.Commands;
using EduRate.Application.Features.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookingsController(IMediator mediator) => _mediator = mediator;

        private int GetUserId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. Create (Student only)
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateBooking(CreateBookingCommand command)
        {
            command.StudentId = GetUserId();
            return Ok(await _mediator.Send(command));
        }

        // 2. Own bookings (Student only)
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyBookings()
            => Ok(await _mediator.Send(new GetMyBookingsQuery { StudentId = GetUserId() }));

        // 3. Cancel + refund (Student only, must own the booking)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var message = await _mediator.Send(new CancelBookingCommand { StudentId = GetUserId(), BookingId = id });
            return Ok(new { message });
        }

        // 4. Mark attendance (Teacher or Admin). Body is a raw boolean, matching the
        // original [FromBody] bool isAttended contract exactly.
        [HttpPatch("{id}/attendance")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> MarkAttendance(int id, [FromBody] bool isAttended)
        {
            var message = await _mediator.Send(new MarkAttendanceCommand { BookingId = id, IsAttended = isAttended });
            return Ok(new { message });
        }
    }
}
