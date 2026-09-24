using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Queries
{
    // Returns raw Session entities (with Teacher included), matching the original GetMySessions action exactly.
    public class GetMySessionsQuery : IRequest<List<Session>>
    {
        public int CenterId { get; set; }
    }

    public class GetMySessionsQueryHandler : IRequestHandler<GetMySessionsQuery, List<Session>>
    {
        private readonly ISessionRepository _sessionRepository;
        public GetMySessionsQueryHandler(ISessionRepository sessionRepository) => _sessionRepository = sessionRepository;

        public async Task<List<Session>> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
        {
            return await _sessionRepository.Query()
                .Where(s => s.CenterId == request.CenterId)
                .Include(s => s.Teacher)
                .ToListAsync(cancellationToken);
        }
    }
}
