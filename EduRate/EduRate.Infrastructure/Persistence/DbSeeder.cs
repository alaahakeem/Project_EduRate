using EduRate.Domain.Entities;
using EduRate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduRate.Infrastructure.Persistence
{
    /// <summary>
    /// The original app's large inline database seeder, moved out of Program.cs verbatim
    /// (same accounts, same random test data, same idempotency checks) so Program.cs can
    /// stay a thin composition root.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                // 1. Apply any pending migrations.
                await context.Database.MigrateAsync();

                // ==========================================
                // Identity seeding
                // ==========================================

                if (await userManager.FindByEmailAsync("admin@edurate.com") == null)
                {
                    var admin = new ApplicationUser { UserName = "admin", Email = "admin@edurate.com", UserType = "Admin" };
                    await userManager.CreateAsync(admin, "Admin@123");
                }

                if (await userManager.FindByEmailAsync("student@edurate.com") == null)
                {
                    var studentUser = new ApplicationUser { UserName = "test_student", Email = "student@edurate.com", UserType = "Student" };
                    var result = await userManager.CreateAsync(studentUser, "Student@123");

                    if (result.Succeeded && !await context.Students.AnyAsync(s => s.Email == "student@edurate.com"))
                    {
                        context.Students.Add(new Student
                        {
                            Name = "Test Student",
                            Email = "student@edurate.com",
                            EducationalStage = (EducationalStage)1,
                            WalletBalance = 500,
                            RewardPoints = 100
                        });
                        await context.SaveChangesAsync();
                    }
                }

                if (await userManager.FindByEmailAsync("teacher@edurate.com") == null)
                {
                    var teacherUser = new ApplicationUser { UserName = "test_teacher", Email = "teacher@edurate.com", UserType = "Teacher" };
                    var result = await userManager.CreateAsync(teacherUser, "Teacher@123");

                    if (result.Succeeded && !await context.Teachers.AnyAsync(t => t.Name == "Test Teacher"))
                    {
                        context.Teachers.Add(new Teacher
                        {
                            Name = "Test Teacher",
                            Bio = "Official Test Teacher Account",
                            TrustScore = 100,
                            YearsOfExperience = 5
                        });
                        await context.SaveChangesAsync();
                    }
                }

                var random = new Random();

                // 2. Subjects
                if (!await context.Subjects.AnyAsync())
                {
                    var subjects = new List<Subject>();
                    var subjectNames = new[] { "Mathematics", "Physics", "Chemistry", "Biology", "English", "Arabic", "History", "Geography", "French", "Philosophy" };

                    foreach (EducationalStage stage in Enum.GetValues(typeof(EducationalStage)))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            subjects.Add(new Subject
                            {
                                Name = subjectNames[random.Next(subjectNames.Length)],
                                EducationalStage = stage
                            });
                        }
                    }
                    await context.Subjects.AddRangeAsync(subjects);
                    await context.SaveChangesAsync();
                }

                // 3. Centers
                if (!await context.Centers.AnyAsync())
                {
                    var centers = new List<Center>
                    {
                        new Center { Name = "Elite Academy", Address = "Dokki, Giza", Latitude = 30.0381, Longitude = 31.2112, IsVerified = true, Description = "Top educational center in Giza" },
                        new Center { Name = "Pioneers Hub", Address = "Nasr City, Cairo", Latitude = 30.0626, Longitude = 31.3283, IsVerified = true, Description = "Air-conditioned and well-equipped" },
                        new Center { Name = "Future Center", Address = "Maadi, Cairo", Latitude = 29.9599, Longitude = 31.2588, IsVerified = true, Description = "Best teachers in town" },
                        new Center { Name = "Success Makers", Address = "Haram, Giza", Latitude = 29.9923, Longitude = 31.1444, IsVerified = true, Description = "Specialized for high school" },
                        new Center { Name = "Summit Academy", Address = "Heliopolis, Cairo", Latitude = 30.0911, Longitude = 31.3196, IsVerified = false, Description = "Newly opened spacious center" }
                    };
                    await context.Centers.AddRangeAsync(centers);
                    await context.SaveChangesAsync();
                }

                // 4. Center images
                if (!await context.CenterImages.AnyAsync())
                {
                    var centerImages = new List<CenterImage>();
                    var centersList = await context.Centers.ToListAsync();

                    foreach (var center in centersList)
                    {
                        centerImages.Add(new CenterImage
                        {
                            CenterId = center.Id,
                            ImageUrl = $"https://via.placeholder.com/800x600?text={center.Name.Replace(" ", "+")}+Main",
                            IsMain = true
                        });

                        centerImages.Add(new CenterImage
                        {
                            CenterId = center.Id,
                            ImageUrl = $"https://via.placeholder.com/800x600?text={center.Name.Replace(" ", "+")}+Sub",
                            IsMain = false
                        });
                    }
                    await context.CenterImages.AddRangeAsync(centerImages);
                    await context.SaveChangesAsync();
                }

                // 5. Teachers
                if (!await context.Teachers.AnyAsync(t => t.Name != "Test Teacher"))
                {
                    var teachers = new List<Teacher>();
                    var firstNames = new[] { "John", "Michael", "David", "James", "Robert", "William", "Sarah", "Emily", "Jessica", "Olivia" };
                    var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
                    var allSubjects = await context.Subjects.ToListAsync();

                    for (int i = 1; i <= 30; i++)
                    {
                        teachers.Add(new Teacher
                        {
                            Name = $"Mr/Ms. {firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}",
                            Bio = "Expert teacher with over 10 years of experience in modern teaching methods.",
                            YearsOfExperience = random.Next(2, 20),
                            TrustScore = random.Next(70, 100),
                            AverageRating = Math.Round((random.NextDouble() * 2) + 3, 1),
                            TotalReviews = random.Next(10, 500),
                            SubjectId = allSubjects[random.Next(allSubjects.Count)].Id
                        });
                    }
                    await context.Teachers.AddRangeAsync(teachers);
                    await context.SaveChangesAsync();
                }

                // 6. TeacherCenters (link teachers to centers)
                if (!await context.TeacherCenters.AnyAsync())
                {
                    var teacherCenters = new List<TeacherCenter>();
                    var teachersList = await context.Teachers.ToListAsync();
                    var centersList = await context.Centers.ToListAsync();

                    foreach (var teacher in teachersList)
                    {
                        int centersCount = random.Next(1, 3);
                        var selectedCenters = centersList.OrderBy(x => random.Next()).Take(centersCount).ToList();

                        foreach (var center in selectedCenters)
                        {
                            teacherCenters.Add(new TeacherCenter
                            {
                                TeacherId = teacher.Id,
                                CenterId = center.Id,
                                Price = random.Next(50, 200),
                                ProfitPercentage = random.Next(30, 60),
                                IsActive = true
                            });
                        }
                    }
                    await context.TeacherCenters.AddRangeAsync(teacherCenters);
                    await context.SaveChangesAsync();
                }

                // 7. Students
                if (!await context.Students.AnyAsync(s => s.Email != "student@edurate.com"))
                {
                    var students = new List<Student>();
                    var studentNames = new[] { "Alex", "Chris", "Daniel", "Matthew", "Anthony", "Sophia", "Isabella", "Mia", "Amelia", "Harper" };

                    for (int i = 1; i <= 50; i++)
                    {
                        double lat = 30.0 + (random.NextDouble() * 0.1);
                        double lng = 31.2 + (random.NextDouble() * 0.1);

                        students.Add(new Student
                        {
                            Name = $"{studentNames[random.Next(studentNames.Length)]} {random.Next(100, 999)}",
                            Email = $"student{i}@edurate.com",
                            EducationalStage = (EducationalStage)random.Next(1, 13),
                            Governorate = "Cairo",
                            Region = "Random District",
                            Latitude = lat,
                            Longitude = lng,
                            WalletBalance = random.Next(100, 1000),
                            RewardPoints = random.Next(0, 500)
                        });
                    }
                    await context.Students.AddRangeAsync(students);
                    await context.SaveChangesAsync();
                }

                // 8. Sessions
                if (!await context.Sessions.AnyAsync())
                {
                    var sessions = new List<Session>();
                    var tcList = await context.TeacherCenters.Include(tc => tc.Teacher).ThenInclude(t => t.Subject).ToListAsync();

                    if (tcList.Any())
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            var tc = tcList[random.Next(tcList.Count)];
                            var startTime = DateTime.Now.AddDays(random.Next(-10, 20)).AddHours(random.Next(8, 20));

                            sessions.Add(new Session
                            {
                                Title = $"{tc.Teacher.Subject?.Name ?? "General"} Revision Session",
                                StartTime = startTime,
                                EndTime = startTime.AddHours(2),
                                Price = tc.Price,
                                EducationalStage = tc.Teacher.Subject?.EducationalStage.ToString() ?? "General",
                                Status = startTime < DateTime.Now ? "Completed" : "Available",
                                CenterId = tc.CenterId,
                                TeacherId = tc.TeacherId
                            });
                        }
                        await context.Sessions.AddRangeAsync(sessions);
                        await context.SaveChangesAsync();
                    }
                }

                // 9. Bookings
                if (!await context.Bookings.AnyAsync())
                {
                    var bookings = new List<Booking>();
                    var studentsList = await context.Students.ToListAsync();
                    var sessionsList = await context.Sessions.ToListAsync();

                    if (studentsList.Any() && sessionsList.Any())
                    {
                        for (int i = 0; i < 300; i++)
                        {
                            var session = sessionsList[random.Next(sessionsList.Count)];
                            var isPast = session.StartTime < DateTime.Now;

                            bookings.Add(new Booking
                            {
                                StudentId = studentsList[random.Next(studentsList.Count)].Id,
                                SessionId = session.Id,
                                BookingDate = session.StartTime.AddDays(-random.Next(1, 5)),
                                Status = "Confirmed",
                                IsAttended = isPast && random.Next(0, 100) > 20
                            });
                        }
                        await context.Bookings.AddRangeAsync(bookings);
                        await context.SaveChangesAsync();
                    }
                }

                // 10. Reviews
                if (!await context.Reviews.AnyAsync())
                {
                    var reviews = new List<Review>();
                    var studentsList = await context.Students.ToListAsync();
                    var teachersList = await context.Teachers.ToListAsync();
                    var centersList = await context.Centers.ToListAsync();
                    var comments = new[] { "Excellent!", "Very good explanation", "Center is a bit crowded but good", "Great experience", "Highly recommended!" };

                    if (studentsList.Any() && teachersList.Any() && centersList.Any())
                    {
                        for (int i = 0; i < 200; i++)
                        {
                            bool isForTeacher = random.Next(0, 2) == 0;

                            reviews.Add(new Review
                            {
                                Rating = random.Next(3, 6),
                                Comment = comments[random.Next(comments.Length)],
                                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60)),
                                IsVerified = true,
                                IsAnonymous = random.Next(0, 2) == 0,
                                StudentId = studentsList[random.Next(studentsList.Count)].Id,
                                TeacherId = isForTeacher ? teachersList[random.Next(teachersList.Count)].Id : null,
                                CenterId = !isForTeacher ? centersList[random.Next(centersList.Count)].Id : null
                            });
                        }
                        await context.Reviews.AddRangeAsync(reviews);
                        await context.SaveChangesAsync();
                    }
                }

                // 11. Messages
                if (!await context.Messages.AnyAsync())
                {
                    var messages = new List<Message>();
                    var studentsList = await context.Students.ToListAsync();
                    var teachersList = await context.Teachers.ToListAsync();
                    var messageContents = new[] { "Hello Teacher, when is the next session?", "Thank you for the great lesson!", "Can you send me the PDF?", "I will be late for the next class.", "Great job in the exam!" };

                    if (studentsList.Any() && teachersList.Any())
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            bool isFromStudent = random.Next(0, 2) == 0;

                            messages.Add(new Message
                            {
                                Content = messageContents[random.Next(messageContents.Length)],
                                SentAt = DateTime.Now.AddDays(-random.Next(1, 30)),
                                IsRead = random.Next(0, 2) == 0,
                                SenderRole = isFromStudent ? "Student" : "Teacher",
                                StudentId = studentsList[random.Next(studentsList.Count)].Id,
                                TeacherId = teachersList[random.Next(teachersList.Count)].Id
                            });
                        }
                        await context.Messages.AddRangeAsync(messages);
                        await context.SaveChangesAsync();
                    }
                }

                // 12. Notifications
                if (!await context.Notifications.AnyAsync())
                {
                    var notifications = new List<Notification>();
                    var studentsList = await context.Students.ToListAsync();
                    var teachersList = await context.Teachers.ToListAsync();

                    if (studentsList.Any() && teachersList.Any())
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            notifications.Add(new Notification
                            {
                                Title = "System Alert",
                                Message = "You have a new update in your schedule.",
                                IsRead = random.Next(0, 2) == 0,
                                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 10)),
                                StudentId = random.Next(0, 2) == 0 ? studentsList[random.Next(studentsList.Count)].Id : null,
                                TeacherId = random.Next(0, 2) == 0 ? teachersList[random.Next(teachersList.Count)].Id : null
                            });
                        }
                        await context.Notifications.AddRangeAsync(notifications);
                        await context.SaveChangesAsync();
                    }
                }

                // 13. Promo codes
                if (!await context.PromoCodes.AnyAsync())
                {
                    var promoCodes = new List<PromoCode>
                    {
                        new PromoCode { Code = "WELCOME20", DiscountPercentage = 20, ExpiryDate = DateTime.Now.AddMonths(1), MaxUsageCount = 100, CurrentUsageCount = 15, IsActive = true },
                        new PromoCode { Code = "SUMMER50", DiscountPercentage = 50, ExpiryDate = DateTime.Now.AddMonths(2), MaxUsageCount = 50, CurrentUsageCount = 49, IsActive = true },
                        new PromoCode { Code = "EDURATE10", DiscountPercentage = 10, ExpiryDate = DateTime.Now.AddMonths(6), MaxUsageCount = 500, CurrentUsageCount = 200, IsActive = true }
                    };
                    await context.PromoCodes.AddRangeAsync(promoCodes);
                    await context.SaveChangesAsync();
                }

                // 14. Student favorites
                if (!await context.StudentFavorites.AnyAsync())
                {
                    var favorites = new List<StudentFavorite>();
                    var studentsList = await context.Students.ToListAsync();
                    var teachersList = await context.Teachers.ToListAsync();
                    var centersList = await context.Centers.ToListAsync();

                    if (studentsList.Any() && teachersList.Any() && centersList.Any())
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            bool isTeacherFavorite = random.Next(0, 2) == 0;

                            favorites.Add(new StudentFavorite
                            {
                                StudentId = studentsList[random.Next(studentsList.Count)].Id,
                                TeacherId = isTeacherFavorite ? teachersList[random.Next(teachersList.Count)].Id : null,
                                CenterId = !isTeacherFavorite ? centersList[random.Next(centersList.Count)].Id : null,
                                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60))
                            });
                        }
                        await context.StudentFavorites.AddRangeAsync(favorites);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
