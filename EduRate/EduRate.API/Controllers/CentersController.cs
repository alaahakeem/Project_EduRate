using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Centers.Commands;
using EduRate.Application.Features.Centers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env; // kept for parity with the original constructor / unimplemented image-upload stub below

        public CentersController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        private int GetProfileId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        #region Center dashboard (implemented via CQRS)

        [HttpGet("my-profile")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> GetMyProfile()
            => Ok(await _mediator.Send(new GetMyProfileQuery { CenterId = GetProfileId() }));

        [HttpPut("my-profile")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
        {
            command.CenterId = GetProfileId();
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("my-sessions")]
        [Authorize(Roles = "Center")]
        public async Task<IActionResult> GetMySessions()
            => Ok(await _mediator.Send(new GetMySessionsQuery { CenterId = GetProfileId() }));

        #endregion

        #region 1. CRUD (implemented via CQRS)

        [HttpGet]
        public async Task<IActionResult> GetCenters([FromQuery] bool onlyVerified = false)
            => Ok(await _mediator.Send(new GetCentersQuery { OnlyVerified = onlyVerified }));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCenter(int id)
            => Ok(await _mediator.Send(new GetCenterQuery { Id = id }));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCenter(CreateCenterCommand command)
        {
            var center = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCenter), new { id = center.Id }, center);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCenter(int id, UpdateCenterCommand command)
        {
            command.CenterId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCenter(int id)
        {
            await _mediator.Send(new DeleteCenterCommand { CenterId = id });
            return NoContent();
        }
        #endregion

        #region 2. Search/filter - UNIMPLEMENTED IN THE ORIGINAL APP, kept exactly as stubs
        [HttpGet("search")]
        public Task<IActionResult> SearchCenters([FromQuery] string name, [FromQuery] string location)
            => Task.FromResult<IActionResult>(Ok());

        [HttpGet("nearby")]
        public Task<IActionResult> GetNearbyCenters([FromQuery] double lat, [FromQuery] double lng)
            => Task.FromResult<IActionResult>(Ok());

        [HttpGet("top")]
        public Task<IActionResult> GetTopCenters()
            => Task.FromResult<IActionResult>(Ok());
        #endregion

        #region 3. Teacher linking - UNIMPLEMENTED IN THE ORIGINAL APP, kept exactly as stubs
        [HttpGet("{id}/teachers")]
        public Task<IActionResult> GetCenterTeachers(int id)
            => Task.FromResult<IActionResult>(Ok());

        [HttpPost("{id}/teachers")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> AddTeacherToCenter(int id, [FromBody] object dto)
            => Task.FromResult<IActionResult>(Ok("Teacher added to center successfully."));

        [HttpPut("{id}/teachers/{teacherId}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> ToggleTeacherStatus(int id, int teacherId)
            => Task.FromResult<IActionResult>(Ok());
        #endregion

        #region 4. Reviews (implemented) + schedule/verify/stats - UNIMPLEMENTED, kept exactly as stubs

        [HttpPost("{id}/reviews")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddCenterReview(int id, AddCenterReviewCommand command)
        {
            command.CenterId = id;
            command.StudentId = GetProfileId();
            var message = await _mediator.Send(command);
            return Ok(message);
        }

        [HttpGet("{id}/schedule")]
        public Task<IActionResult> GetCenterSchedule(int id)
            => Task.FromResult<IActionResult>(Ok());

        [HttpPatch("{id}/verify")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> VerifyCenter(int id)
            => Task.FromResult<IActionResult>(NoContent());

        [HttpGet("{id}/stats")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> GetCenterStats(int id)
            => Task.FromResult<IActionResult>(Ok());
        #endregion

        #region 5. Gallery - UNIMPLEMENTED IN THE ORIGINAL APP, kept exactly as stubs
        [HttpGet("{id}/images")]
        public Task<IActionResult> GetCenterImages(int id)
            => Task.FromResult<IActionResult>(Ok());

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> AddCenterImage(int id, IFormFile file, [FromQuery] bool isMain = false)
            => Task.FromResult<IActionResult>(Ok());
        #endregion
    }
}
