using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Queries
{
    public class GetCenterQuery : IRequest<CenterDetailsDto>
    {
        public int Id { get; set; }
    }

    public class GetCenterQueryHandler : IRequestHandler<GetCenterQuery, CenterDetailsDto>
    {
        private readonly ICenterRepository _centerRepository;
        public GetCenterQueryHandler(ICenterRepository centerRepository) => _centerRepository = centerRepository;

        public async Task<CenterDetailsDto> Handle(GetCenterQuery request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.Query()
                .Select(c => new CenterDetailsDto { Id = c.Id, Name = c.Name, Location = c.Address, Latitude = c.Latitude, Longitude = c.Longitude, IsVerified = c.IsVerified })
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (center == null) throw new NotFoundException("Center not found.");
            return center;
        }
    }
}
