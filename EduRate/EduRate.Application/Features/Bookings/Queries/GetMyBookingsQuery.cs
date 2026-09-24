using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Bookings.Queries
{
    public class GetMyBookingsQuery : IRequest<List<BookingReaddDto>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, List<BookingReaddDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        public GetMyBookingsQueryHandler(IBookingRepository bookingRepository) => _bookingRepository = bookingRepository;

        public async Task<List<BookingReaddDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.Query()
                .Include(b => b.Student)
                .Include(b => b.Session).ThenInclude(s => s.Teacher)
                .Include(b => b.Session).ThenInclude(s => s.Center)
                .Where(b => b.StudentId == request.StudentId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingReaddDto
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    IsAttended = b.IsAttended,
                    Status = b.Status,
                    SessionId = b.Session.Id,
                    SessionTitle = b.Session.Title,
                    SessionStartTime = b.Session.StartTime,
                    SessionEndTime = b.Session.EndTime,
                    SessionPrice = b.Session.Price,
                    TeacherName = b.Session.Teacher.Name,
                    CenterName = b.Session.Center.Name,
                    StudentId = b.Student.Id,
                    StudentName = b.Student.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
