using EduRate.Application.Common.Exceptions;
using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Application.Features.Reviews.Commands
{
    public class AddOrUpdateReviewCommand : IRequest<string>
    {
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? CenterId { get; set; }
        public int? SessionId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
    }

    public class AddOrUpdateReviewCommandValidator : AbstractValidator<AddOrUpdateReviewCommand>
    {
        public AddOrUpdateReviewCommandValidator()
        {
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون من 1 إلى 5");
            RuleFor(x => x).Must(x => (x.TeacherId.HasValue ? 1 : 0) + (x.CenterId.HasValue ? 1 : 0) + (x.SessionId.HasValue ? 1 : 0) == 1)
                .WithMessage("يجب تحديد جهة تقييم واحدة فقط (مدرس، سنتر، أو حصة).");
        }
    }

    public class AddOrUpdateReviewCommandHandler : IRequestHandler<AddOrUpdateReviewCommand, string>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AddOrUpdateReviewCommandHandler(
            IBookingRepository bookingRepository,
            IReviewRepository reviewRepository,
            ITeacherRepository teacherRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(AddOrUpdateReviewCommand request, CancellationToken cancellationToken)
        {
            bool hasAttended = false;

            if (request.SessionId != null)
            {
                hasAttended = await _bookingRepository.Query()
                    .AnyAsync(b => b.StudentId == request.StudentId && b.SessionId == request.SessionId && b.IsAttended == true, cancellationToken);
            }
            else if (request.TeacherId != null)
            {
                hasAttended = await _bookingRepository.Query()
                    .Include(b => b.Session)
                    .AnyAsync(b => b.StudentId == request.StudentId && b.Session.TeacherId == request.TeacherId && b.IsAttended == true, cancellationToken);
            }
            else if (request.CenterId != null)
            {
                hasAttended = await _bookingRepository.Query()
                    .Include(b => b.Session)
                    .AnyAsync(b => b.StudentId == request.StudentId && b.Session.CenterId == request.CenterId && b.IsAttended == true, cancellationToken);
            }

            if (!hasAttended)
                throw new BadRequestException("عذراً، لا يمكنك التقييم إلا بعد حضور حصة فعلية.");

            var existingReview = await _reviewRepository.Query()
                .FirstOrDefaultAsync(r => r.StudentId == request.StudentId &&
                                      ((request.TeacherId != null && r.TeacherId == request.TeacherId) ||
                                       (request.CenterId != null && r.CenterId == request.CenterId) ||
                                       (request.SessionId != null && r.SessionId == request.SessionId)), cancellationToken);

            if (existingReview != null)
            {
                var timePassed = DateTime.Now - existingReview.CreatedAt;

                if (timePassed.TotalMinutes > 15)
                    throw new BadRequestException("عذراً، لا يمكنك تعديل التقييم بعد مرور 15 دقيقة من نشره الأصلي.");

                existingReview.Rating = request.Rating;
                existingReview.Comment = request.Comment;
                existingReview.IsAnonymous = request.IsAnonymous;
            }
            else
            {
                var newReview = new Review
                {
                    StudentId = request.StudentId,
                    TeacherId = request.TeacherId,
                    CenterId = request.CenterId,
                    SessionId = request.SessionId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    IsAnonymous = request.IsAnonymous,
                    IsVerified = true,
                    CreatedAt = DateTime.Now
                };
                _reviewRepository.Add(newReview);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.TeacherId != null)
                await UpdateTeacherAverageRatingAsync(request.TeacherId.Value, cancellationToken);

            if (request.TeacherId.HasValue)
            {
                await _notificationService.SendToTeacherAsync(request.TeacherId.Value, "تقييم جديد! \ud83c\udf1f", $"قام أحد الطلاب بإضافة تقييم جديد لك بـ {request.Rating} نجوم.");
            }
            else if (request.CenterId.HasValue)
            {
                await _notificationService.SendToCenterAsync(request.CenterId.Value, "تقييم جديد للسنتر! \ud83c\udfe2", $"حصل السنتر على تقييم جديد بـ {request.Rating} نجوم من أحد الطلاب.");
            }

            return "تم حفظ التقييم بنجاح.";
        }

        private async Task UpdateTeacherAverageRatingAsync(int teacherId, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null) return;

            var reviews = await _reviewRepository.Query().Where(r => r.TeacherId == teacherId).ToListAsync(cancellationToken);

            if (reviews.Any())
            {
                teacher.AverageRating = reviews.Average(r => r.Rating);
                teacher.TotalReviews = reviews.Count;
            }
            else
            {
                teacher.AverageRating = 0;
                teacher.TotalReviews = 0;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
