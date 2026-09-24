using EduRate.Domain.Entities;

namespace EduRate.Application.Common.Interfaces
{
    // One repository per aggregate/DbSet. Most are plain markers over the generic
    // IRepository<T> - handlers compose their existing LINQ via Query(). Only add
    // a bespoke method here when several handlers would otherwise repeat the same
    // non-trivial query (kept minimal on purpose - see AGENT instructions: avoid
    // unnecessary repositories/abstractions).

    public interface IStudentRepository : IRepository<Student> { }

    public interface ITeacherRepository : IRepository<Teacher> { }

    public interface ICenterRepository : IRepository<Center> { }

    public interface ISessionRepository : IRepository<Session> { }

    public interface IBookingRepository : IRepository<Booking> { }

    public interface IReviewRepository : IRepository<Review> { }

    public interface IMessageRepository : IRepository<Message> { }

    public interface INotificationRepository : IRepository<Notification> { }

    public interface IStudentFavoriteRepository : IRepository<StudentFavorite> { }

    public interface IPromoCodeRepository : IRepository<PromoCode> { }

    public interface ISubjectRepository : IRepository<Subject> { }

    public interface ITeacherCenterRepository : IRepository<TeacherCenter> { }

    public interface ICenterImageRepository : IRepository<CenterImage> { }
}
