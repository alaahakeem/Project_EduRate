using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Centers.Commands
{
    public class UpdateCenterCommand : IRequest
    {
        public int CenterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UpdateCenterCommandValidator : AbstractValidator<UpdateCenterCommand>
    {
        public UpdateCenterCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
        }
    }

    public class UpdateCenterCommandHandler : IRequestHandler<UpdateCenterCommand>
    {
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCenterCommandHandler(ICenterRepository centerRepository, IUnitOfWork unitOfWork)
        {
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCenterCommand request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.GetByIdAsync(request.CenterId, cancellationToken);
            if (center == null) throw new NotFoundException("Center not found.");

            center.Name = request.Name;
            center.Address = request.Location;
            center.Latitude = request.Latitude;
            center.Longitude = request.Longitude;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
