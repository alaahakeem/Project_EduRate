using EduRate.Application.Common.Interfaces;
using EduRate.Domain.Entities;
using EduRate.Infrastructure.Persistence;

namespace EduRate.Infrastructure.Persistence.Repositories
{
    // Thin concrete repositories - one per aggregate/DbSet, matching the interfaces
    // declared in Application. All behaviour lives in the generic Repository<T> base.

    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context) { }
    }

    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(AppDbContext context) : base(context) { }
    }

    public class CenterRepository : Repository<Center>, ICenterRepository
    {
        public CenterRepository(AppDbContext context) : base(context) { }
    }

    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(AppDbContext context) : base(context) { }
    }

    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }
    }

    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }
    }

    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }
    }

    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }
    }

    public class StudentFavoriteRepository : Repository<StudentFavorite>, IStudentFavoriteRepository
    {
        public StudentFavoriteRepository(AppDbContext context) : base(context) { }
    }

    public class PromoCodeRepository : Repository<PromoCode>, IPromoCodeRepository
    {
        public PromoCodeRepository(AppDbContext context) : base(context) { }
    }

    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context) { }
    }

    public class TeacherCenterRepository : Repository<TeacherCenter>, ITeacherCenterRepository
    {
        public TeacherCenterRepository(AppDbContext context) : base(context) { }
    }

    public class CenterImageRepository : Repository<CenterImage>, ICenterImageRepository
    {
        public CenterImageRepository(AppDbContext context) : base(context) { }
    }
}
