using System.Text.Json;
using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator) => _mediator = mediator;

        private int GetStudentId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. Request a Paymob payment link (Student only)
        [HttpPost("charge")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ChargeWallet([FromBody] ChargeWalletViaGatewayCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        // 2. Paymob webhook callback - must stay anonymous so Paymob's server can call it.
        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobWebhook([FromBody] JsonElement payload)
        {
            await _mediator.Send(new PaymobWebhookCommand { Payload = payload });
            return Ok();
        }
    }
}
