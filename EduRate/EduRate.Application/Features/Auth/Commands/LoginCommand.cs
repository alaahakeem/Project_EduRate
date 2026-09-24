using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("A valid email is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICenterRepository _centerRepository;

        public LoginCommandHandler(
            IIdentityService identityService,
            IJwtTokenGenerator jwtTokenGenerator,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            ICenterRepository centerRepository)
        {
            _identityService = identityService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _centerRepository = centerRepository;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);

            if (user == null || !await _identityService.CheckPasswordAsync(request.Email, request.Password))
            {
                throw new UnauthorizedException("Invalid Email or Password");
            }

            int profileId = 0;

            if (user.UserType == "Student")
            {
                var student = await _studentRepository.Query().FirstOrDefaultAsync(s => s.Email == user.Email, cancellationToken);
                if (student != null) profileId = student.Id;
            }
            else if (user.UserType == "Teacher")
            {
                // Teacher has no Email column, so we match by username as the original code did.
                var teacher = await _teacherRepository.Query().FirstOrDefaultAsync(t => t.Name == user.UserName, cancellationToken);
                if (teacher != null) profileId = teacher.Id;
            }
            else if (user.UserType == "Center")
            {
                var center = await _centerRepository.Query().FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
                if (center != null) profileId = center.Id;
            }

            var (token, expiration) = _jwtTokenGenerator.GenerateToken(user.UserName, user.Email, user.UserType ?? "User", profileId);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration,
                UserType = user.UserType,
                ProfileId = profileId
            };
        }
    }
}
