using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<BookingReaddDto>
    {
        public int StudentId { get; set; }
        public int SessionId { get; set; }
    }

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingReaddDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CreateBookingCommandHandler(
            IStudentRepository studentRepository,
            ISessionRepository sessionRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _studentRepository = studentRepository;
            _sessionRepository = sessionRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<BookingReaddDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null) throw new BadRequestException("الطالب غير موجود.");

            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null) throw new BadRequestException("الحصة غير موجودة.");

            if (session.Status == "Cancelled")
                throw new BadRequestException("لا يمكن الحجز في حصة ملغية.");

            if (session.StartTime < DateTime.Now)
                throw new BadRequestException("لا يمكن الحجز في حصة انتهى موعدها أو بدأت بالفعل.");

            if (student.WalletBalance < session.Price)
                throw new BadRequestException("رصيد المحفظة لا يكفي لحجز هذه الحصة. يرجى الشحن أولاً.");

            var alreadyBooked = await _bookingRepository.Query()
                .AnyAsync(b => b.StudentId == request.StudentId && b.SessionId == request.SessionId && b.Status != "Cancelled", cancellationToken);

            if (alreadyBooked)
                throw new BadRequestException("لقد قمت بحجز هذه الحصة مسبقاً.");

            var hasTimeConflict = await _bookingRepository.Query()
                .Include(b => b.Session)
                .AnyAsync(b => b.StudentId == request.StudentId &&
                               b.Status != "Cancelled" &&
                               b.Session.StartTime < session.EndTime &&
                               session.StartTime < b.Session.EndTime, cancellationToken);

            if (hasTimeConflict)
                throw new BadRequestException("لديك حجز آخر يتعارض مع توقيت هذه الحصة.");

            student.WalletBalance -= session.Price;

            var booking = new Booking
            {
                StudentId = request.StudentId,
                SessionId = request.SessionId,
                BookingDate = DateTime.Now,
                Status = "Pending",
                IsAttended = false
            };

            _bookingRepository.Add(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdBooking = await _bookingRepository.Query()
                .Include(b => b.Student)
                .Include(b => b.Session).ThenInclude(s => s.Teacher)
                .Include(b => b.Session).ThenInclude(s => s.Center)
                .FirstOrDefaultAsync(b => b.Id == booking.Id, cancellationToken);

            if (createdBooking != null)
            {
                await _notificationService.SendToStudentAsync(request.StudentId, "تم تأكيد حجزك! \ud83c\udf89", $"تم تأكيد حجزك بنجاح وخصم {session.Price} من محفظتك للحصة '{createdBooking.Session.Title}'.");
                await _notificationService.SendToTeacherAsync(createdBooking.Session.Teacher.Id, "حجز جديد! \ud83d\udcc5", $"قام الطالب {createdBooking.Student.Name} بحجز مقعد في حصتك '{createdBooking.Session.Title}'.");
                await _notificationService.SendToCenterAsync(createdBooking.Session.Center.Id, "تأكيد حجز جديد \ud83c\udfe2", $"تم حجز مقعد جديد للطالب {createdBooking.Student.Name} في حصة المدرس {createdBooking.Session.Teacher.Name}.");
            }

            return new BookingReaddDto
            {
                Id = createdBooking!.Id,
                BookingDate = createdBooking.BookingDate,
                IsAttended = createdBooking.IsAttended,
                Status = createdBooking.Status,
                SessionId = createdBooking.Session.Id,
                SessionTitle = createdBooking.Session.Title,
                SessionStartTime = createdBooking.Session.StartTime,
                SessionEndTime = createdBooking.Session.EndTime,
                SessionPrice = createdBooking.Session.Price,
                TeacherName = createdBooking.Session.Teacher.Name,
                CenterName = createdBooking.Session.Center.Name,
                StudentId = createdBooking.Student.Id,
                StudentName = createdBooking.Student.Name
            };
        }
    }
}
