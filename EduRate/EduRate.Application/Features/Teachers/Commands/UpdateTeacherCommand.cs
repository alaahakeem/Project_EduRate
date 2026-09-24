using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Teachers.Commands
{
    public class UpdateTeacherCommand : IRequest
    {
        public int TeacherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string DemoVideoUrl { get; set; } = string.Empty;
    }

    public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
    {
        public UpdateTeacherCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeacherCommandHandler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null) throw new NotFoundException("المدرس غير موجود");

            teacher.Name = request.Name;
            teacher.SubjectId = request.SubjectId;
            teacher.Bio = request.Bio;
            teacher.YearsOfExperience = request.YearsOfExperience;
            teacher.DemoVideoUrl = request.DemoVideoUrl;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
