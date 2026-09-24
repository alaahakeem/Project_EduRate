using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Auth.Commands
{
    /// <summary>
    /// Registers a new Identity user and creates the matching Student/Teacher/Center profile row,
    /// exactly as the original AuthController.Register action did.
    /// </summary>
    public class RegisterCommand : IRequest<string>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // Student, Teacher, Center

        // Student-only fields
        public int? EducationalStage { get; set; }
        public string? Governorate { get; set; }
        public string? Region { get; set; }

        // Teacher-only field
        public int? SubjectId { get; set; }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("A valid email is required");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.UserType).NotEmpty().WithMessage("UserType is required (e.g., Student, Teacher, Center)");
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IIdentityService identityService,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            ICenterRepository centerRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _identityService.FindByEmailAsync(request.Email);
            if (userExists != null)
                throw new BadRequestException("User with this email already exists!");

            var createResult = await _identityService.CreateUserAsync(request.Email, request.Username, request.Password, request.UserType);
            if (!createResult.Succeeded)
            {
                throw new BadRequestException("User creation failed! " + string.Join(" ", createResult.Errors));
            }

            if (request.UserType == "Student")
            {
                if (!request.EducationalStage.HasValue || string.IsNullOrEmpty(request.Governorate) || string.IsNullOrEmpty(request.Region))
                {
                    await _identityService.DeleteUserAsync(createResult.UserId);
                    throw new BadRequestException("Educational Stage, Governorate, and Region are required for students!");
                }

                var student = new Student
                {
                    Name = request.Name,
                    Email = request.Email,
                    EducationalStage = (EducationalStage)request.EducationalStage.Value,
                    WalletBalance = 0,
                    RewardPoints = 0,
                    Governorate = request.Governorate,
                    Region = request.Region
                };
                _studentRepository.Add(student);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else if (request.UserType == "Teacher")
            {
                if (!request.SubjectId.HasValue)
                {
                    await _identityService.DeleteUserAsync(createResult.UserId);
                    throw new BadRequestException("SubjectId is required for teachers!");
                }

                var teacher = new Teacher
                {
                    Name = request.Name,
                    Bio = "New Teacher",
                    TrustScore = 0,
                    YearsOfExperience = 0,
                    SubjectId = request.SubjectId.Value
                };
                _teacherRepository.Add(teacher);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else if (request.UserType == "Center")
            {
                var center = new Center
                {
                    Name = request.Name,
                    Description = "New Center",
                    Address = "Not specified",
                    IsVerified = false,
                    UserId = createResult.UserId
                };
                _centerRepository.Add(center);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return "User registered successfully and profile created!";
        }
    }
}
