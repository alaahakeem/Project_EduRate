using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Students.Commands
{
    public class UpdateLocationCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public string Governorate { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
    {
        public UpdateLocationCommandValidator()
        {
            RuleFor(x => x.Governorate).NotEmpty();
            RuleFor(x => x.Region).NotEmpty();
        }
    }

    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, string>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLocationCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            student.Governorate = request.Governorate;
            student.Region = request.Region;
            student.Latitude = request.Latitude;
            student.Longitude = request.Longitude;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return "Location updated successfully.";
        }
    }
}
