using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Commands
{
    // Returns the raw (updated) Center entity, matching the original UpdateMyProfile action exactly.
    public class UpdateMyProfileCommand : IRequest<Center>
    {
        public int CenterId { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Center>
    {
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMyProfileCommandHandler(ICenterRepository centerRepository, IUnitOfWork unitOfWork)
        {
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Center> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.Query().FirstOrDefaultAsync(c => c.Id == request.CenterId, cancellationToken);
            if (center == null) throw new NotFoundException("Center not found.");

            center.Name = request.Name ?? center.Name;
            center.Address = request.Location ?? center.Address;
            center.Latitude = request.Latitude ?? center.Latitude;
            center.Longitude = request.Longitude ?? center.Longitude;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return center;
        }
    }
}
