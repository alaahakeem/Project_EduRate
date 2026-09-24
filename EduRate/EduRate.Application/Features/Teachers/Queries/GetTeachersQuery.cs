using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    public class GetTeachersQuery : IRequest<List<TeacherReadDto>> { }

    public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, List<TeacherReadDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        public GetTeachersQueryHandler(ITeacherRepository teacherRepository) => _teacherRepository = teacherRepository;

        public async Task<List<TeacherReadDto>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
        {
            return await _teacherRepository.Query()
                .Include(t => t.Subject)
                .OrderByDescending(t => t.AverageRating)
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
                .ToListAsync(cancellationToken);
        }
    }
}
