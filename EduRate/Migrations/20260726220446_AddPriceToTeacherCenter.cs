using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduRate.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToTeacherCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TeacherCenters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2026, 7, 24, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4081));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2026, 7, 26, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4088));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "SentAt",
                value: new DateTime(2026, 7, 26, 20, 4, 46, 520, DateTimeKind.Local).AddTicks(4146));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "SentAt",
                value: new DateTime(2026, 7, 26, 21, 4, 46, 520, DateTimeKind.Local).AddTicks(4151));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 25, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4116));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 26, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4122));

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "JoinDate", "Price" },
                values: new object[] { new DateTime(2026, 1, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4012), 120.0m });

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 2, 1 },
                columns: new[] { "JoinDate", "Price" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4023), 150.0m });

            migrationBuilder.UpdateData(
                table: "TeacherCenters",
                keyColumns: new[] { "CenterId", "TeacherId" },
                keyValues: new object[] { 1, 2 },
                columns: new[] { "JoinDate", "Price" },
                values: new object[] { new DateTime(2025, 7, 27, 1, 4, 46, 520, DateTimeKind.Local).AddTicks(4028), 100.0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "TeacherCenters");

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
    }
}
