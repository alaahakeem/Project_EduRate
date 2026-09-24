using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Sessions.Queries
{
    public class GetSessionBookingsQuery : IRequest<List<SessionBookingItemDto>>
    {
        public int SessionId { get; set; }
    }

    public class GetSessionBookingsQueryHandler : IRequestHandler<GetSessionBookingsQuery, List<SessionBookingItemDto>>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUser;

        public GetSessionBookingsQueryHandler(ISessionRepository sessionRepository, IBookingRepository bookingRepository, ICurrentUserService currentUser)
        {
            _sessionRepository = sessionRepository;
            _bookingRepository = bookingRepository;
            _currentUser = currentUser;
        }

        public async Task<List<SessionBookingItemDto>> Handle(GetSessionBookingsQuery request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null) throw new NotFoundException("الحصة غير موجودة");

            if (_currentUser.IsInRole("Teacher") && session.TeacherId != _currentUser.ProfileId)
                throw new ForbiddenAccessException("لا تملك صلاحية عرض كشف هذه الحصة.");

            return await _bookingRepository.Query()
                .Include(b => b.Student)
                .Where(b => b.SessionId == request.SessionId)
                .Select(b => new SessionBookingItemDto
                {
                    BookingId = b.Id,
                    StudentName = b.Student.Name,
                    BookingDate = b.BookingDate,
                    Status = b.Status
                })
                .ToListAsync(cancellationToken);
        }
    }
}
