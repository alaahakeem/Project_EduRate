using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class AddAllModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EducationalLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherCenters",
                columns: table => new
                {
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    CenterId = table.Column<int>(type: "int", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfitPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherCenters", x => new { x.TeacherId, x.CenterId });
                    table.ForeignKey(
                        name: "FK_TeacherCenters_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherCenters_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Centers",
                columns: new[] { "Id", "Address", "Description", "IsVerified", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "شارع المحطة", "قاعات مكيفة ومجهزة", true, 30.0444, 31.235700000000001, "سنتر الأوائل" },
                    { 2, "وسط البلد", "مراجعات نهائية مكثفة", false, 30.033300000000001, 31.2333, "سنتر النخبة" }
                });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Comment", "CreatedAt", "StudentId" },
                values: new object[] { "Excellent explanation!", new DateTime(2026, 7, 24, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9443), 1 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Comment", "CreatedAt", "StudentId", "TeacherId" },
                values: new object[] { "Very good.", new DateTime(2026, 7, 25, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9447), 2, 2 });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "EducationalLevel", "Email", "Governorate", "Name", "Region" },
                values: new object[,]
                {
                    { 1, "Senior 2", "ahmed@example.com", "القاهرة", "Ahmed Khaled", "المعادي" },
                    { 2, "Senior 3", "mona@example.com", "الجيزة", "Mona Sayed", "الدقي" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "IsAttended", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 23, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9419), true, 1, 1 },
                    { 2, new DateTime(2026, 7, 25, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9424), false, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "IsRead", "SenderRole", "SentAt", "StudentId", "TeacherId" },
                values: new object[,]
                {
                    { 1, "يا مستر ممكن تشرح الجزء الأخير تاني؟", true, "Student", new DateTime(2026, 7, 25, 22, 12, 23, 335, DateTimeKind.Local).AddTicks(9466), 1, 1 },
                    { 2, "أكيد، راجع الفيديو اللي نزلته وهتفهمه.", false, "Teacher", new DateTime(2026, 7, 25, 23, 12, 23, 335, DateTimeKind.Local).AddTicks(9470), 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "TeacherCenters",
                columns: new[] { "CenterId", "TeacherId", "IsActive", "JoinDate", "ProfitPercentage" },
                values: new object[,]
                {
                    { 1, 1, true, new DateTime(2026, 1, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9362), 70.5m },
                    { 2, 1, true, new DateTime(2026, 5, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9371), 60.0m },
                    { 1, 2, false, new DateTime(2025, 7, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9375), 80.0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudentId",
                table: "Reviews",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StudentId",
                table: "Bookings",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TeacherId",
                table: "Bookings",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_StudentId",
                table: "Messages",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TeacherId",
                table: "Messages",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCenters_CenterId",
                table: "TeacherCenters",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Students_StudentId",
                table: "Reviews",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Students_StudentId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "TeacherCenters");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_StudentId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Comment", "CreatedAt", "StudentName" },
                values: new object[] { "شرح ممتاز جداً", new DateTime(2026, 7, 24, 2, 25, 37, 778, DateTimeKind.Local).AddTicks(7792), "طالب مجهول" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Comment", "CreatedAt", "StudentName", "TeacherId" },
                values: new object[] { "كويس بس الحصة طويلة", new DateTime(2026, 7, 25, 2, 25, 37, 778, DateTimeKind.Local).AddTicks(7798), "أحمد خالد", 1 });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AverageRating", "Bio", "Name", "Subject", "TotalReviews", "TrustScore", "YearsOfExperience" },
                values: new object[,]
                {
                    { 3, 3.0, "Makes math fun and easy to understand.", "Eng. Tarek Hassan", "Mathematics", 1, 92.0, 8 },
                    { 4, 5.0, "Focuses on practical experiments and problem-solving.", "Dr. Sarah Ibrahim", "Chemistry", 1, 98.0, 12 }
                });
        }
    }
}
