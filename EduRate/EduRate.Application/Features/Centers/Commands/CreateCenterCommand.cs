using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EduRate.Application.Features.Centers.Commands
{
    // Returns the raw created Center entity, matching the original CreateCenter action exactly.
    public class CreateCenterCommand : IRequest<Center>
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class CreateCenterCommandValidator : AbstractValidator<CreateCenterCommand>
    {
        public CreateCenterCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
        }
    }

    public class CreateCenterCommandHandler : IRequestHandler<CreateCenterCommand, Center>
    {
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCenterCommandHandler(ICenterRepository centerRepository, IUnitOfWork unitOfWork)
        {
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Center> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
        {
            var center = new Center
            {
                Name = request.Name,
                Address = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsVerified = false
            };

            _centerRepository.Add(center);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return center;
        }
    }
}
