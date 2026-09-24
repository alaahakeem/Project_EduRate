using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;

namespace EduRate.Application.Features.Centers.Commands
{
    public class DeleteCenterCommand : IRequest
    {
        public int CenterId { get; set; }
    }

    public class DeleteCenterCommandHandler : IRequestHandler<DeleteCenterCommand>
    {
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCenterCommandHandler(ICenterRepository centerRepository, IUnitOfWork unitOfWork)
        {
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCenterCommand request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.GetByIdAsync(request.CenterId, cancellationToken);
            if (center == null) throw new NotFoundException("Center not found.");

            _centerRepository.Remove(center);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
