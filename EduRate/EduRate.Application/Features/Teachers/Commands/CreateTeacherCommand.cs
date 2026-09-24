using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Teachers.Commands
{
    // Returns the created Teacher entity, matching the original PostTeacher action
    // (which returned the raw entity via CreatedAtAction rather than a DTO).
    public class CreateTeacherCommand : IRequest<Teacher>
    {
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string DemoVideoUrl { get; set; } = string.Empty;
    }

    public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
    {
        public CreateTeacherCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SubjectId).GreaterThan(0);
        }
    }

    public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Teacher>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Teacher> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            var newTeacher = new Teacher
            {
                Name = request.Name,
                SubjectId = request.SubjectId,
                Bio = request.Bio,
                YearsOfExperience = request.YearsOfExperience,
                DemoVideoUrl = request.DemoVideoUrl,
                TrustScore = 100,
                AverageRating = 0,
                TotalReviews = 0
            };

            _teacherRepository.Add(newTeacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newTeacher;
        }
    }
}
