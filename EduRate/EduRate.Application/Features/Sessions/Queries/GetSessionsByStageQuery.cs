using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Queries
{
    public class GetSessionsByStageQuery : IRequest<List<SessionReadDto>>
    {
        public string Stage { get; set; } = string.Empty;
    }

    public class GetSessionsByStageQueryHandler : IRequestHandler<GetSessionsByStageQuery, List<SessionReadDto>>
    {
        private readonly ISessionRepository _sessionRepository;
        public GetSessionsByStageQueryHandler(ISessionRepository sessionRepository) => _sessionRepository = sessionRepository;

        public async Task<List<SessionReadDto>> Handle(GetSessionsByStageQuery request, CancellationToken cancellationToken)
        {
            return await _sessionRepository.Query()
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .Where(s => s.EducationalStage == request.Stage && s.Status == "Available")
                .OrderBy(s => s.StartTime)
                .Select(s => new SessionReadDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Price = s.Price,
                    EducationalStage = s.EducationalStage,
                    Status = s.Status,
                    CenterName = s.Center.Name,
                    TeacherName = s.Teacher.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
