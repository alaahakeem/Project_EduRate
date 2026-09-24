using EduRate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // تسجيل كل الجداول في الداتابيز
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CenterImage> CenterImages { get; set; }
        public DbSet<TeacherCenter> TeacherCenters { get; set; } // الجدول الوسيط
        public DbSet<StudentFavorite> StudentFavorites { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Session)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // 1. تعريف المفتاح الأساسي المركب لجدول TeacherCenter
            modelBuilder.Entity<TeacherCenter>()
                .HasKey(tc => new { tc.TeacherId, tc.CenterId });

            // ضبط الـ Precision لنسبة الأرباح
            modelBuilder.Entity<TeacherCenter>()
                .Property(tc => tc.ProfitPercentage)
                .HasPrecision(5, 2);

            // ضبط نوع البيانات لعمود السعر
            modelBuilder.Entity<TeacherCenter>()
                .Property(tc => tc.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}