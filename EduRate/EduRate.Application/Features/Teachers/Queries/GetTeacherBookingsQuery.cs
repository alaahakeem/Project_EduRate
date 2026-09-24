using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Teachers.Queries
{
    public class GetTeacherBookingsQuery : IRequest<List<BookingReadDto>>
    {
        public int TeacherId { get; set; }
    }

    public class GetTeacherBookingsQueryHandler : IRequestHandler<GetTeacherBookingsQuery, List<BookingReadDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        public GetTeacherBookingsQueryHandler(IBookingRepository bookingRepository) => _bookingRepository = bookingRepository;

        public async Task<List<BookingReadDto>> Handle(GetTeacherBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.Query()
                .Where(b => b.Session.TeacherId == request.TeacherId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingReadDto
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    StudentName = b.Student.Name
                })
                .ToListAsync(cancellationToken);

            if (!bookings.Any()) throw new NotFoundException("لا يوجد حجوزات حالياً");
            return bookings;
        }
    }
}
