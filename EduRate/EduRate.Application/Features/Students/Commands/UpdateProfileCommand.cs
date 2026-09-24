using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Students.Commands
{
    public class UpdateProfileCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ParentPhoneNumber { get; set; }
        public int? EducationalStage { get; set; }
    }

    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, string>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            student.Name = request.Name;
            // Matches original behavior: ParentPhoneNumber/EducationalStage updates were commented out server-side.

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "Profile updated successfully.";
        }
    }
}
