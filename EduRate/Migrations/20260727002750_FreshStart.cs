using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class FreshStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Centers",
                columns: new[] { "Id", "Address", "Description", "IsVerified", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "شارع المحطة", "قاعات مكيفة ومجهزة", true, 30.0444, 31.235700000000001, "سنتر الأوائل" },
                    { 2, "وسط البلد", "مراجعات نهائية مكثفة", false, 30.033300000000001, 31.2333, "سنتر النخبة" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "EducationalLevel", "Email", "Governorate", "Name", "Region" },
                values: new object[,]
                {
                    { 1, "Senior 2", "ahmed@example.com", "القاهرة", "Ahmed Khaled", "المعادي" },
                    { 2, "Senior 3", "mona@example.com", "الجيزة", "Mona Sayed", "الدقي" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AverageRating", "Bio", "Name", "Subject", "TotalReviews", "TrustScore", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 4.5, "Expert in simplifying Physics concepts.", "Mr. Mohammed Ahmed", "Physics", 2, 95.0, 10 },
                    { 2, 5.0, "Specialized in foundational English and grammar.", "Mr. Mahmoud Ali", "English", 1, 88.0, 5 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "IsAttended", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 24, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4081), true, 1, 1 },
                    { 2, new DateTime(2026, 7, 26, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4088), false, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "IsRead", "SenderRole", "SentAt", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, "يا مستر ممكن تشرح الجزء الأخير تاني؟", true, "Student", new DateTime(2026, 7, 26, 20, 4, 46, 520, DateTimeKind.Local).AddTicks(4146), 1, 1 },
                    { 2, "أكيد، راجع الفيديو اللي نزلته وهتفهمه.", false, "Teacher", new DateTime(2026, 7, 26, 21, 4, 46, 520, DateTimeKind.Local).AddTicks(4151), 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "IPAddress", "IsVerified", "Rating", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, "Excellent explanation!", new DateTime(2026, 7, 25, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4116), "192.168.1.1", true, 5, 1, 1 },
                    { 2, "Very good.", new DateTime(2026, 7, 26, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4122), "192.168.1.5", true, 4, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "TeacherCenters",
                columns: new[] { "CenterId", "TeacherId", "IsActive", "JoinDate", "Price", "ProfitPercentage" },
                values: new object[,]
                {
                    { 1, 1, true, new DateTime(2026, 1, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4012), 120.0m, 70.5m },
                    { 2, 1, true, new DateTime(2026, 5, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4023), 150.0m, 60.0m },
                    { 1, 2, false, new DateTime(2025, 7, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4028), 100.0m, 80.0m }
                });
        }
    }
}
