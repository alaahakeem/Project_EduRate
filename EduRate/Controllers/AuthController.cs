using EduRate.Data;
using EduRate.DTOs;
using EduRate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EduRate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest("User with this email already exists!");

            ApplicationUser user = new ApplicationUser()
            {
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = dto.Username,
                UserType = dto.UserType
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);
                return BadRequest(new { Message = "User creation failed!", Errors = errors });
            }

            // =======================================================
            // 💡 تسجيل بروفايل الطالب
            // =======================================================
            if (dto.UserType == "Student")
            {
                if (!dto.EducationalStage.HasValue || string.IsNullOrEmpty(dto.Governorate) || string.IsNullOrEmpty(dto.Region))
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest("Educational Stage, Governorate, and Region are required for students!");
                }

                var student = new Student
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    EducationalStage = (EducationalStage)dto.EducationalStage.Value,
                    WalletBalance = 0,
                    RewardPoints = 0,
                    Governorate = dto.Governorate,
                    Region = dto.Region
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
            // =======================================================
            // 💡 تسجيل بروفايل المدرس
            // =======================================================
            else if (dto.UserType == "Teacher")
            {
                // لازم نأكد إن المادة مبعوتة عشان الداتابيز متضربش إيرور
                if (!dto.SubjectId.HasValue)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest("SubjectId is required for teachers!");
                }

                var teacher = new Teacher
                {
                    Name = dto.Name,
                    Bio = "New Teacher",
                    TrustScore = 0,
                    YearsOfExperience = 0,
                    SubjectId = dto.SubjectId.Value
                };
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
            }
            // =======================================================
            // 💡 التعديل هنا: تسجيل بروفايل السنتر
            // =======================================================
            else if (dto.UserType == "Center")
            {
                var center = new Center
                {
                    Name = dto.Name,
                    Description = "New Center",
                    Address = "لم يتم التحديد",
                    IsVerified = false,
                    UserId = user.Id // ربطنا السنتر بحسابه
                };
                _context.Centers.Add(center);
                await _context.SaveChangesAsync();
            }

            return Ok("User registered successfully and profile created!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                int profileId = 0;

                // 💡 هنجيب البروفايل بتاع الطالب أو المدرس أو السنتر عشان نرجع الـ ID بتاعه
                if (user.UserType == "Student")
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
                    if (student != null) profileId = student.Id;
                }
                else if (user.UserType == "Teacher")
                {
                    // بما إن المدرس ملوش عمود Email في الموديل، هنبحث عنه بالاسم
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Name == user.UserName);
                    if (teacher != null) profileId = teacher.Id;
                }
                // 💡 التعديل هنا: استرجاع الـ ID بتاع السنتر
                else if (user.UserType == "Center")
                {
                    var center = await _context.Centers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (center != null) profileId = center.Id;
                }

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    // 💡 التعديل هنا: استخدمنا ClaimTypes.Role عشان [Authorize] يفهمه!
                    new Claim(ClaimTypes.Role, user.UserType ?? "User"),
                    new Claim("ProfileId", profileId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["JWT:DurationInDays"])),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    userType = user.UserType,
                    profileId = profileId
                });
            }
            return Unauthorized("Invalid Email or Password");
        }
    }
}