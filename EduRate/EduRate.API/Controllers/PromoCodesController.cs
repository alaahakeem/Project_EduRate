using EduRate.Application.Features.PromoCodes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class PromoCodesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PromoCodesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePromoCode([FromBody] ValidatePromoCodeQuery query)
            => Ok(await _mediator.Send(query));
    }
}
