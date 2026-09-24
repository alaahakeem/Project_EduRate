using EduRate.Domain.Entities;
using EduRate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Center> Centers { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<CenterImage> CenterImages { get; set; } = null!;
        public DbSet<TeacherCenter> TeacherCenters { get; set; } = null!; // join table
        public DbSet<StudentFavorite> StudentFavorites { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<PromoCode> PromoCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Session)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Composite primary key for the TeacherCenter join table.
            modelBuilder.Entity<TeacherCenter>()
                .HasKey(tc => new { tc.TeacherId, tc.CenterId });

            modelBuilder.Entity<TeacherCenter>()
                .Property(tc => tc.ProfitPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<TeacherCenter>()
                .Property(tc => tc.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
