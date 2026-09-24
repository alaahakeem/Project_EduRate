using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewsAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Teachers_TeacherId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CenterId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SessionId",
                table: "Reviews",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Sessions_SessionId",
                table: "Reviews",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Teachers_TeacherId",
                table: "Reviews",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Sessions_SessionId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Teachers_TeacherId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_SessionId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CenterId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Centers_CenterId",
                table: "Reviews",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Teachers_TeacherId",
                table: "Reviews",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
