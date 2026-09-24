using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Queries
{
    // Returns the raw Center entity, matching the original GetMyProfile action exactly.
    public class GetMyProfileQuery : IRequest<Center>
    {
        public int CenterId { get; set; }
    }

    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Center>
    {
        private readonly ICenterRepository _centerRepository;
        public GetMyProfileQueryHandler(ICenterRepository centerRepository) => _centerRepository = centerRepository;

        public async Task<Center> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.Query().FirstOrDefaultAsync(c => c.Id == request.CenterId, cancellationToken);
            if (center == null) throw new NotFoundException("لم يتم العثور على بيانات السنتر.");
            return center;
        }
    }
}
