using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Students.Queries
{
    public class GetMyBookingsQuery : IRequest<List<StudentBookingDto>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, List<StudentBookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        public GetMyBookingsQueryHandler(IBookingRepository bookingRepository) => _bookingRepository = bookingRepository;

        public async Task<List<StudentBookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.Query()
                .Include(b => b.Session)
                .Where(b => b.StudentId == request.StudentId)
                .Select(b => new StudentBookingDto
                {
                    BookingId = b.Id,
                    SessionTitle = b.Session.Title,
                    StartTime = b.Session.StartTime,
                    EndTime = b.Session.EndTime,
                    Status = b.Status,
                    IsAttended = b.IsAttended
                })
                .ToListAsync(cancellationToken);
        }
    }
}
