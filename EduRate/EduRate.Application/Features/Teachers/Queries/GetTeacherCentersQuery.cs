using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    // Returns raw TeacherCenter entities, matching the original GetTeacherCenters action exactly.
    public class GetTeacherCentersQuery : IRequest<List<TeacherCenter>>
    {
        public int TeacherId { get; set; }
    }

    public class GetTeacherCentersQueryHandler : IRequestHandler<GetTeacherCentersQuery, List<TeacherCenter>>
    {
        private readonly ITeacherCenterRepository _teacherCenterRepository;
        public GetTeacherCentersQueryHandler(ITeacherCenterRepository teacherCenterRepository) => _teacherCenterRepository = teacherCenterRepository;

        public async Task<List<TeacherCenter>> Handle(GetTeacherCentersQuery request, CancellationToken cancellationToken)
        {
            return await _teacherCenterRepository.Query()
                .Where(tc => tc.TeacherId == request.TeacherId)
                .ToListAsync(cancellationToken);
        }
    }
}
