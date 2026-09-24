using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    TrustScore = table.Column<double>(type: "float", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    TotalReviews = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AverageRating", "Bio", "Name", "Subject", "TotalReviews", "TrustScore", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, 4.5, "Expert in simplifying Physics concepts.", "Mr. Mohammed Ahmed", "Physics", 2, 95.0, 10 },
                    { 2, 5.0, "Specialized in foundational English and grammar.", "Mr. Mahmoud Ali", "English", 1, 88.0, 5 },
                    { 3, 3.0, "Makes math fun and easy to understand.", "Eng. Tarek Hassan", "Mathematics", 1, 92.0, 8 },
                    { 4, 5.0, "Focuses on practical experiments and problem-solving.", "Dr. Sarah Ibrahim", "Chemistry", 1, 98.0, 12 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "IPAddress", "IsVerified", "Rating", "StudentName", "TeacherId" },
                values: new object[,]
                {
                    { 1, "شرح ممتاز جداً", new DateTime(2026, 7, 24, 2, 25, 37, 778, DateTimeKind.Local).AddTicks(7792), "192.168.1.1", true, 5, "طالب مجهول", 1 },
                    { 2, "كويس بس الحصة طويلة", new DateTime(2026, 7, 25, 2, 25, 37, 778, DateTimeKind.Local).AddTicks(7798), "192.168.1.5", true, 4, "أحمد خالد", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TeacherId",
                table: "Reviews",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
