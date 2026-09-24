using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Reviews.Commands;
using EduRate.Application.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReviewsController(IMediator mediator) => _mediator = mediator;

        private int GetStudentId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. Add or update a review (Student only)
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddOrUpdateReview(AddOrUpdateReviewCommand command)
        {
            command.StudentId = GetStudentId();
            var message = await _mediator.Send(command);
            return Ok(new { message });
        }

        // Read endpoints - open to everyone
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherReviews(int teacherId)
            => Ok(await _mediator.Send(new GetTeacherReviewsQuery { TeacherId = teacherId }));

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSessionReviews(int sessionId)
            => Ok(await _mediator.Send(new GetSessionReviewsQuery { SessionId = sessionId }));

        [HttpGet("center/{centerId}")]
        public async Task<IActionResult> GetCenterReviews(int centerId)
            => Ok(await _mediator.Send(new GetCenterReviewsQuery { CenterId = centerId }));
    }
}
