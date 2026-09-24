using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduRate.Data;
using EduRate.DTOs;
using EduRate.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")] // 💡 قفل الحماية: للطلاب فقط
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 💡 Helper Method: استخراج الـ ID من التوكن
        // ==========================================
        private int? GetStudentIdFromToken()
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (!string.IsNullOrEmpty(profileIdClaim) && int.TryParse(profileIdClaim, out int id))
            {
                return id;
            }
            return null;
        }

        // ==========================================
        // 1. Profile Management
        // ==========================================
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetProfile()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            var dto = new StudentProfileDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                EducationalStage = student.EducationalStage.ToString(),
                Governorate = student.Governorate,
                Region = student.Region,
                // ParentPhoneNumber = student.ParentPhoneNumber, // تأكدي إن الخاصية دي موجودة في جدول الطلاب
                WalletBalance = student.WalletBalance,
                RewardPoints = student.RewardPoints
            };
            return Ok(dto);
        }

        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            student.Name = dto.Name;
            // student.ParentPhoneNumber = dto.ParentPhoneNumber;
            // student.EducationalStage = dto.EducationalStage;

            await _context.SaveChangesAsync();
            return Ok("Profile updated successfully.");
        }

        // ==========================================
        // 2. Location Management
        // ==========================================
        [HttpPut("my-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            student.Governorate = dto.Governorate;
            student.Region = dto.Region;
            student.Latitude = dto.Latitude;
            student.Longitude = dto.Longitude;

            await _context.SaveChangesAsync();
            return Ok("Location updated successfully.");
        }

        // ==========================================
        // 3. Wallet & Rewards
        // ==========================================
        [HttpGet("my-wallet")]
        public async Task<IActionResult> GetWalletInfo()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            return Ok(new WalletInfoDto // تأكدي إن الـ DTO ده موجود
            {
                WalletBalance = student.WalletBalance,
                RewardPoints = student.RewardPoints
            });
        }

        [HttpPut("my-wallet/charge")]
        public async Task<IActionResult> ChargeWallet([FromBody] ChargeWalletDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            if (dto.Amount <= 0) return BadRequest("Amount must be greater than zero.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            student.WalletBalance += dto.Amount;
            await _context.SaveChangesAsync();
            return Ok($"Wallet charged successfully. New balance: {student.WalletBalance}");
        }

        [HttpPut("my-rewards/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound("Student not found.");

            if (student.RewardPoints < dto.Points)
                return BadRequest("Not enough reward points.");

            // كل 100 نقطة بـ 10 جنيه كمثال للـ Business Logic
            decimal cashValue = (dto.Points / 100m) * 10m;

            student.RewardPoints -= dto.Points;
            
            student.WalletBalance += cashValue; // حولناها لـ double حسب نوع المتغير في الداتابيز

            await _context.SaveChangesAsync();
            return Ok($"Points redeemed for {cashValue} EGP. New balance: {student.WalletBalance}");
        }

        // ==========================================
        // 4. Bookings
        // ==========================================
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetBookings()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var bookings = await _context.Bookings
                .Include(b => b.Session)
                .Where(b => b.StudentId == studentId)
                .Select(b => new StudentBookingDto // تأكدي إن الـ DTO ده موجود
                {
                    BookingId = b.Id,
                    SessionTitle = b.Session.Title,
                    StartTime = b.Session.StartTime,
                    EndTime = b.Session.EndTime,
                    Status = b.Status,
                    IsAttended = b.IsAttended
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // ==========================================
        // 5. Reviews
        // ==========================================
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetReviews()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var reviews = await _context.Reviews
                .Include(r => r.Teacher)
                .Include(r => r.Center)
                .Where(r => r.StudentId == studentId)
                .Select(r => new StudentReviewDto // تأكدي إن الـ DTO ده موجود
                {
                    ReviewId = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    TargetName = r.TeacherId != null ? r.Teacher.Name : r.Center.Name
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // ==========================================
        // 6. Notifications
        // ==========================================
        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var notifications = await _context.Notifications
                .Where(n => n.StudentId == studentId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new StudentNotificationDto // تأكدي إن الـ DTO ده موجود
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPut("my-notifications/mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var unreadNotifications = await _context.Notifications
                .Where(n => n.StudentId == studentId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok("All notifications marked as read.");
        }

        // ==========================================
        // 7. Favorites
        // ==========================================
        [HttpGet("my-favorites")]
        public async Task<IActionResult> GetFavorites()
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var favorites = await _context.StudentFavorites
                .Include(f => f.Teacher)
                .Include(f => f.Center)
                .Where(f => f.StudentId == studentId)
                .Select(f => new StudentFavoriteDto // تأكدي إن الـ DTO ده موجود
                {
                    FavoriteId = f.Id,
                    TeacherId = f.TeacherId,
                    TeacherName = f.Teacher != null ? f.Teacher.Name : null,
                    CenterId = f.CenterId,
                    CenterName = f.Center != null ? f.Center.Name : null
                })
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpPost("my-favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            if (dto.TeacherId == null && dto.CenterId == null)
                return BadRequest("You must provide either a TeacherId or a CenterId.");

            // نمنع إضافة المدرس أو السنتر مرتين
            var exists = await _context.StudentFavorites.AnyAsync(f =>
                f.StudentId == studentId &&
                ((dto.TeacherId != null && f.TeacherId == dto.TeacherId) ||
                 (dto.CenterId != null && f.CenterId == dto.CenterId)));

            if (exists) return BadRequest("This item is already in favorites.");

            var favorite = new StudentFavorite
            {
                StudentId = (int)studentId,
                TeacherId = dto.TeacherId,
                CenterId = dto.CenterId,
                CreatedAt = System.DateTime.Now
            };

            _context.StudentFavorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("Added to favorites successfully.");
        }

        [HttpDelete("my-favorites/{favoriteId}")]
        public async Task<IActionResult> RemoveFavorite(int favoriteId) // هنا بنحتاج بس رقم المفضلة اللي هنحذفها
        {
            var studentId = GetStudentIdFromToken();
            if (studentId == null) return Unauthorized("Invalid token.");

            var favorite = await _context.StudentFavorites
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.StudentId == studentId);

            if (favorite == null) return NotFound("Favorite not found.");

            _context.StudentFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("Removed from favorites successfully.");
        }
    }
}