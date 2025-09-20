using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClinicD2bSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Code", "CreatedAtUtc", "Description", "IsActive", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { 1, "CARD", new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6689), "Khoa Tim mạch", true, "Cardiology", null },
                    { 2, "NEUR", new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6692), "Khoa Thần kinh", true, "Neurology", null },
                    { 3, "DERM", new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6694), "Khoa Da liễu", true, "Dermatology", null }
                });

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(4955), new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(4953) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(4754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 74, DateTimeKind.Utc).AddTicks(8160));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 74, DateTimeKind.Utc).AddTicks(8165));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 74, DateTimeKind.Utc).AddTicks(8166));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 74, DateTimeKind.Utc).AddTicks(8167));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 9, 18, 12, 31, 19, 439, DateTimeKind.Utc).AddTicks(8777), new DateTime(2025, 9, 18, 12, 31, 19, 439, DateTimeKind.Utc).AddTicks(8774) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 18, 12, 31, 19, 439, DateTimeKind.Utc).AddTicks(8655));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 18, 12, 31, 19, 441, DateTimeKind.Utc).AddTicks(2456));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 18, 12, 31, 19, 441, DateTimeKind.Utc).AddTicks(2458));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 18, 12, 31, 19, 441, DateTimeKind.Utc).AddTicks(2460));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 18, 12, 31, 19, 441, DateTimeKind.Utc).AddTicks(2462));
        }
    }
}
