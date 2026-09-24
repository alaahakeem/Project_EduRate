using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Subjects.Commands
{
    public class AddSubjectCommand : IRequest<SubjectDto>
    {
        public string Name { get; set; } = string.Empty;
        public string EducationalStage { get; set; } = string.Empty;
    }

    public class AddSubjectCommandValidator : AbstractValidator<AddSubjectCommand>
    {
        public AddSubjectCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.EducationalStage).NotEmpty();
        }
    }

    public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, SubjectDto>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddSubjectCommandHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubjectDto> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = new Subject
            {
                Name = request.Name,
                EducationalStage = Enum.Parse<EducationalStage>(request.EducationalStage, true)
            };

            _subjectRepository.Add(subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                EducationalStage = subject.EducationalStage.ToString()
            };
        }
    }
}
