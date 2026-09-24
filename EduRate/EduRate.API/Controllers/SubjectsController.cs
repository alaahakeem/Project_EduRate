using EduRate.Application.Features.Subjects.Commands;
using EduRate.Application.Features.Subjects.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SubjectsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
            => Ok(await _mediator.Send(new GetAllSubjectsQuery()));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubject(AddSubjectCommand command)
        {
            var subjectDto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllSubjects), new { id = subjectDto.Id }, subjectDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectCommand command)
        {
            command.SubjectId = id;
            var message = await _mediator.Send(command);
            return Ok(new { message });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var message = await _mediator.Send(new DeleteSubjectCommand { SubjectId = id });
            return Ok(new { message });
        }
    }
}
