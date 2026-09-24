using EduRate.Application.Common.Exceptions;
using EduRate.Domain.Entities;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Subjects.Commands
{
    public class UpdateSubjectCommand : IRequest<string>
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EducationalStage { get; set; } = string.Empty;
    }

    public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
    {
        public UpdateSubjectCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.EducationalStage).NotEmpty();
        }
    }

    public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, string>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubjectCommandHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null) throw new NotFoundException("المادة غير موجودة.");

            subject.Name = request.Name;
            subject.EducationalStage = Enum.Parse<EducationalStage>(request.EducationalStage, true);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "تم تعديل المادة بنجاح.";
        }
    }
}
