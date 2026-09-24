using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Reviews.Queries
{
    public class GetTeacherReviewsQuery : IRequest<List<ReviewReaddDto>>
    {
        public int TeacherId { get; set; }
    }

    public class GetTeacherReviewsQueryHandler : IRequestHandler<GetTeacherReviewsQuery, List<ReviewReaddDto>>
    {
        private readonly IReviewRepository _reviewRepository;
        public GetTeacherReviewsQueryHandler(IReviewRepository reviewRepository) => _reviewRepository = reviewRepository;

        public async Task<List<ReviewReaddDto>> Handle(GetTeacherReviewsQuery request, CancellationToken cancellationToken)
        {
            var editDeadline = DateTime.Now.AddMinutes(-15);
            return await _reviewRepository.Query()
                .Include(r => r.Student)
                .Where(r => r.TeacherId == request.TeacherId)
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
                }).ToListAsync(cancellationToken);
        }
    }
}
