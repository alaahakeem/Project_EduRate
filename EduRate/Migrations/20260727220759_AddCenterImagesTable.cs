using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class AddCenterImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DemoVideoUrl",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CenterId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CenterId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CenterImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    CenterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterImages_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CenterId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CenterId",
                table: "Reviews",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CenterId",
                table: "Bookings",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterImages_CenterId",
                table: "CenterImages",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CenterId",
                table: "Sessions",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Centers_CenterId",
                table: "Bookings",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Centers_CenterId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "CenterImages");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CenterId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CenterId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DemoVideoUrl",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Bookings");
        }
    }
}
