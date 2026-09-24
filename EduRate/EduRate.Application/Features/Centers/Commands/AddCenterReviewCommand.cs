using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Centers.Commands
{
    // NOTE: this is a separate, simpler review pathway from Reviews.AddOrUpdateReviewCommand -
    // it was already a second, less-validated way to review a center in the original app
    // (CentersController.AddCenterReview vs ReviewsController.AddOrUpdateReview). Preserved as-is.
    public class AddCenterReviewCommand : IRequest<string>
    {
        public int CenterId { get; set; }
        public int StudentId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class AddCenterReviewCommandValidator : AbstractValidator<AddCenterReviewCommand>
    {
        public AddCenterReviewCommandValidator()
        {
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        }
    }

    public class AddCenterReviewCommandHandler : IRequestHandler<AddCenterReviewCommand, string>
    {
        private readonly ICenterRepository _centerRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddCenterReviewCommandHandler(ICenterRepository centerRepository, IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
        {
            _centerRepository = centerRepository;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(AddCenterReviewCommand request, CancellationToken cancellationToken)
        {
            if (!await _centerRepository.Query().AnyAsync(c => c.Id == request.CenterId, cancellationToken))
                throw new NotFoundException("Center not found.");

            var review = new Review
            {
                CenterId = request.CenterId,
                StudentId = request.StudentId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now
            };

            _reviewRepository.Add(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return "Review added successfully.";
        }
    }
}
