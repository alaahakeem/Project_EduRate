using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;

namespace EduRate.Application.Features.Students.Queries
{
    public class GetMyProfileQuery : IRequest<StudentProfileDto>
    {
        public int StudentId { get; set; }
    }

    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, StudentProfileDto>
    {
        private readonly IStudentRepository _studentRepository;
        public GetMyProfileQueryHandler(IStudentRepository studentRepository) => _studentRepository = studentRepository;

        public async Task<StudentProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new NotFoundException("Student not found.");

            return new StudentProfileDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                EducationalStage = student.EducationalStage.ToString(),
                Governorate = student.Governorate,
                Region = student.Region,
                WalletBalance = student.WalletBalance,
                RewardPoints = student.RewardPoints
            };
        }
    }
}
