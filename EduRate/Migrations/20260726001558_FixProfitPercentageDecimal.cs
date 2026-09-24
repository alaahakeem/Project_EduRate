using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class FixProfitPercentageDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitPercentage",
                table: "TeacherCenters",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2026, 7, 23, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6779));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2026, 7, 25, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6785));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "SentAt",
                value: new DateTime(2026, 7, 25, 22, 15, 58, 561, DateTimeKind.Local).AddTicks(6830));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "SentAt",
                value: new DateTime(2026, 7, 25, 23, 15, 58, 561, DateTimeKind.Local).AddTicks(6834));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 24, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6803));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 25, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6808));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 1 },
                column: "JoinDate",
                value: new DateTime(2026, 1, 26, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6727));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 2, 1 },
                column: "JoinDate",
                value: new DateTime(2026, 5, 26, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6735));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 2 },
                column: "JoinDate",
                value: new DateTime(2025, 7, 26, 3, 15, 58, 561, DateTimeKind.Local).AddTicks(6739));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProfitPercentage",
                table: "TeacherCenters",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2026, 7, 23, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9419));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2026, 7, 25, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9424));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "SentAt",
                value: new DateTime(2026, 7, 25, 22, 12, 23, 335, DateTimeKind.Local).AddTicks(9466));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "SentAt",
                value: new DateTime(2026, 7, 25, 23, 12, 23, 335, DateTimeKind.Local).AddTicks(9470));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 24, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9443));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 25, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9447));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 1 },
                column: "JoinDate",
                value: new DateTime(2026, 1, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9362));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 2, 1 },
                column: "JoinDate",
                value: new DateTime(2026, 5, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9371));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 2 },
                column: "JoinDate",
                value: new DateTime(2025, 7, 26, 3, 12, 23, 335, DateTimeKind.Local).AddTicks(9375));
        }
    }
}
