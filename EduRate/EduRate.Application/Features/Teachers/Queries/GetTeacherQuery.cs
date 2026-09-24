using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    public class GetTeacherQuery : IRequest<TeacherReadDto>
    {
        public int Id { get; set; }
    }

    public class GetTeacherQueryHandler : IRequestHandler<GetTeacherQuery, TeacherReadDto>
    {
        private readonly ITeacherRepository _teacherRepository;
        public GetTeacherQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

        public async Task<TeacherReadDto> Handle(GetTeacherQuery request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.Query()
                .Include(t => t.Subject)
                .Where(t => t.Id == request.Id)
                .Select(t => new TeacherReadDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    SubjectName = t.Subject != null ? t.Subject.Name : "Not specified",
                    Bio = t.Bio,
                    YearsOfExperience = t.YearsOfExperience,
                    DemoVideoUrl = t.DemoVideoUrl,
                    TrustScore = t.TrustScore,
                    AverageRating = t.AverageRating,
                    TotalReviews = t.TotalReviews
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher == null) throw new NotFoundException("المدرس غير موجود");
            return teacher;
        }
    }
}
