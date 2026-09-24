using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Queries
{
    public class GetSessionsQuery : IRequest<List<SessionReadDto>> { }

    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<SessionReadDto>>
    {
        private readonly ISessionRepository _sessionRepository;
        public GetSessionsQueryHandler(ISessionRepository sessionRepository) => _sessionRepository = sessionRepository;

        public async Task<List<SessionReadDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            return await _sessionRepository.Query()
                .Include(s => s.Teacher)
                .Include(s => s.Center)
                .Where(s => s.Status == "Available")
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
