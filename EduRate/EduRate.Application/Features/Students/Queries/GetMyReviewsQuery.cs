using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Queries
{
    public class GetMyReviewsQuery : IRequest<List<StudentReviewDto>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, List<StudentReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        public GetMyReviewsQueryHandler(IReviewRepository reviewRepository) => _reviewRepository = reviewRepository;

        public async Task<List<StudentReviewDto>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
        {
            return await _reviewRepository.Query()
                .Include(r => r.Teacher)
                .Include(r => r.Center)
                .Where(r => r.StudentId == request.StudentId)
                .Select(r => new StudentReviewDto
                {
                    ReviewId = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    TargetName = r.TeacherId != null ? r.Teacher!.Name : (r.Center != null ? r.Center.Name : null)
                })
                .ToListAsync(cancellationToken);
        }
    }
}
