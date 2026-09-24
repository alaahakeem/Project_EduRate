using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Bookings.Commands
{
    public class CancelBookingCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public int BookingId { get; set; }
    }

    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, string>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _bookingRepository = bookingRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.Query()
                .Include(b => b.Session)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.StudentId == request.StudentId, cancellationToken);

            if (booking == null) throw new NotFoundException("الحجز غير موجود أو لا تملك صلاحية إلغائه.");

            if (booking.Status == "Cancelled")
                throw new BadRequestException("الحجز ملغي بالفعل.");

            if (booking.Session.StartTime <= DateTime.Now)
                throw new BadRequestException("لا يمكن إلغاء الحجز واسترداد المبلغ لأن الحصة قد بدأت أو انتهت بالفعل.");

            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            student!.WalletBalance += booking.Session.Price;

            booking.Status = "Cancelled";
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.SendToStudentAsync(request.StudentId, "تم إلغاء الحجز", $"تم إلغاء الحجز وإرجاع {booking.Session.Price} لمحفظتك.");

            return "تم إلغاء الحجز بنجاح واسترداد المبلغ.";
        }
    }
}
