using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;

namespace EduRate.Application.Features.Bookings.Commands
{
    public class MarkAttendanceCommand : IRequest<string>
    {
        public int BookingId { get; set; }
        public bool IsAttended { get; set; }
    }

    public class MarkAttendanceCommandHandler : IRequestHandler<MarkAttendanceCommand, string>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAttendanceCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null) throw new NotFoundException("الحجز غير موجود.");

            if (booking.Status == "Cancelled")
                throw new BadRequestException("لا يمكن تسجيل حضور لحجز ملغي.");

            booking.IsAttended = request.IsAttended;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.IsAttended ? "تم تسجيل الحضور بنجاح." : "تم إلغاء الحضور.";
        }
    }
}
