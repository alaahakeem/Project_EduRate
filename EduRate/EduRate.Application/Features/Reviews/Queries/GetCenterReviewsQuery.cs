using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Reviews.Queries
{
    public class GetCenterReviewsQuery : IRequest<List<ReviewReaddDto>>
    {
        public int CenterId { get; set; }
    }

    public class GetCenterReviewsQueryHandler : IRequestHandler<GetCenterReviewsQuery, List<ReviewReaddDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICenterRepository _centerRepository;

        public GetCenterReviewsQueryHandler(IReviewRepository reviewRepository, ICenterRepository centerRepository)
        {
            _reviewRepository = reviewRepository;
            _centerRepository = centerRepository;
        }

        public async Task<List<ReviewReaddDto>> Handle(GetCenterReviewsQuery request, CancellationToken cancellationToken)
        {
            var centerExists = await _centerRepository.Query().AnyAsync(c => c.Id == request.CenterId, cancellationToken);
            if (!centerExists) throw new NotFoundException("السنتر غير موجود");

            var editDeadline = DateTime.Now.AddMinutes(-15);
            var reviews = await _reviewRepository.Query()
                .Include(r => r.Student)
                .Where(r => r.CenterId == request.CenterId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewReaddDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    IsAnonymous = r.IsAnonymous,
                    StudentName = r.IsAnonymous ? "طالب مجهول" : r.Student.Name,
                    CanEdit = r.CreatedAt >= editDeadline
                })
                .ToListAsync(cancellationToken);

            if (!reviews.Any()) throw new NotFoundException("لا يوجد تقييمات لهذا السنتر حتى الآن");
            return reviews;
        }
    }
}
