using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Queries
{
    public class GetCentersQuery : IRequest<List<CenterDto>>
    {
        public bool OnlyVerified { get; set; } = false;
    }

    public class GetCentersQueryHandler : IRequestHandler<GetCentersQuery, List<CenterDto>>
    {
        private readonly ICenterRepository _centerRepository;
        public GetCentersQueryHandler(ICenterRepository centerRepository) => _centerRepository = centerRepository;

        public async Task<List<CenterDto>> Handle(GetCentersQuery request, CancellationToken cancellationToken)
        {
            var query = _centerRepository.Query();
            if (request.OnlyVerified) query = query.Where(c => c.IsVerified);

            return await query
                .Select(c => new CenterDto { Id = c.Id, Name = c.Name, Location = c.Address, IsVerified = c.IsVerified })
                .ToListAsync(cancellationToken);
        }
    }
}
