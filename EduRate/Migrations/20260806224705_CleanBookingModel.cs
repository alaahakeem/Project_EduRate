using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class CleanBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Centers_CenterId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Teachers_TeacherId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CenterId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TeacherId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CenterId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CenterId",
                table: "Bookings",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TeacherId",
                table: "Bookings",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Centers_CenterId",
                table: "Bookings",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Teachers_TeacherId",
                table: "Bookings",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
