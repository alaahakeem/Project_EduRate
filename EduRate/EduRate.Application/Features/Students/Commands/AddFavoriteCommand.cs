using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Commands
{
    public class AddFavoriteCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? CenterId { get; set; }
    }

    public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
    {
        public AddFavoriteCommandValidator()
        {
            RuleFor(x => x).Must(x => x.TeacherId != null || x.CenterId != null)
                .WithMessage("You must provide either a TeacherId or a CenterId.");
        }
    }

    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, string>
    {
        private readonly IStudentFavoriteRepository _studentFavoriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddFavoriteCommandHandler(IStudentFavoriteRepository studentFavoriteRepository, IUnitOfWork unitOfWork)
        {
            _studentFavoriteRepository = studentFavoriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var exists = await _studentFavoriteRepository.Query().AnyAsync(f =>
                f.StudentId == request.StudentId &&
                ((request.TeacherId != null && f.TeacherId == request.TeacherId) ||
                 (request.CenterId != null && f.CenterId == request.CenterId)), cancellationToken);

            if (exists) throw new BadRequestException("This item is already in favorites.");

            var favorite = new StudentFavorite
            {
                StudentId = request.StudentId,
                TeacherId = request.TeacherId,
                CenterId = request.CenterId,
                CreatedAt = DateTime.Now
            };

            _studentFavoriteRepository.Add(favorite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "Added to favorites successfully.";
        }
    }
}
