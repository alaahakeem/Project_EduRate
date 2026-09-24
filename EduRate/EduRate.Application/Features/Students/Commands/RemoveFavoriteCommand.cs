using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Commands
{
    public class RemoveFavoriteCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public int FavoriteId { get; set; }
    }

    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, string>
    {
        private readonly IStudentFavoriteRepository _studentFavoriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveFavoriteCommandHandler(IStudentFavoriteRepository studentFavoriteRepository, IUnitOfWork unitOfWork)
        {
            _studentFavoriteRepository = studentFavoriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var favorite = await _studentFavoriteRepository.Query()
                .FirstOrDefaultAsync(f => f.Id == request.FavoriteId && f.StudentId == request.StudentId, cancellationToken);

            if (favorite == null) throw new NotFoundException("Favorite not found.");

            _studentFavoriteRepository.Remove(favorite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "Removed from favorites successfully.";
        }
    }
}
