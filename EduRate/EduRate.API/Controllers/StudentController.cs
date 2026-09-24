using EduRate.Application.Common.Exceptions;
using EduRate.Application.Features.Students.Commands;
using EduRate.Application.Features.Students.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduRate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StudentController(IMediator mediator) => _mediator = mediator;

        private int GetStudentId()
        {
            var claim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out int id)) return id;
            throw new UnauthorizedException("Invalid token.");
        }

        // 1. Profile
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetProfile()
            => Ok(await _mediator.Send(new GetMyProfileQuery { StudentId = GetStudentId() }));

        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        // 2. Location
        [HttpPut("my-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        // 3. Wallet & rewards
        [HttpGet("my-wallet")]
        public async Task<IActionResult> GetWalletInfo()
            => Ok(await _mediator.Send(new GetWalletInfoQuery { StudentId = GetStudentId() }));

        [HttpPut("my-wallet/charge")]
        public async Task<IActionResult> ChargeWallet([FromBody] ChargeWalletCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("my-rewards/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        // 4. Bookings
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetBookings()
            => Ok(await _mediator.Send(new GetMyBookingsQuery { StudentId = GetStudentId() }));

        // 5. Reviews
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetReviews()
            => Ok(await _mediator.Send(new GetMyReviewsQuery { StudentId = GetStudentId() }));

        // 6. Notifications
        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetNotifications()
            => Ok(await _mediator.Send(new GetMyNotificationsQuery { StudentId = GetStudentId() }));

        [HttpPut("my-notifications/mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
            => Ok(await _mediator.Send(new MarkAllNotificationsAsReadCommand { StudentId = GetStudentId() }));

        // 7. Favorites
        [HttpGet("my-favorites")]
        public async Task<IActionResult> GetFavorites()
            => Ok(await _mediator.Send(new GetMyFavoritesQuery { StudentId = GetStudentId() }));

        [HttpPost("my-favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteCommand command)
        {
            command.StudentId = GetStudentId();
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("my-favorites/{favoriteId}")]
        public async Task<IActionResult> RemoveFavorite(int favoriteId)
            => Ok(await _mediator.Send(new RemoveFavoriteCommand { StudentId = GetStudentId(), FavoriteId = favoriteId }));
    }
}
