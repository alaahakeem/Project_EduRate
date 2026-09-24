using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    public class GetTeacherStatsQuery : IRequest<TeacherStatsDto?>
    {
        public int TeacherId { get; set; }
    }

    public class GetTeacherStatsQueryHandler : IRequestHandler<GetTeacherStatsQuery, TeacherStatsDto?>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITeacherRepository _teacherRepository;

        public GetTeacherStatsQueryHandler(IBookingRepository bookingRepository, ITeacherRepository teacherRepository)
        {
            _bookingRepository = bookingRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<TeacherStatsDto?> Handle(GetTeacherStatsQuery request, CancellationToken cancellationToken)
        {
            var totalBookings = await _bookingRepository.Query().CountAsync(b => b.Session.TeacherId == request.TeacherId, cancellationToken);

            return await _teacherRepository.Query()
                .Where(t => t.Id == request.TeacherId)
                .Select(t => new TeacherStatsDto
                {
                    TotalBookings = totalBookings,
                    TotalReviews = t.TotalReviews,
                    AverageRating = t.AverageRating,
                    TrustScore = t.TrustScore
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
