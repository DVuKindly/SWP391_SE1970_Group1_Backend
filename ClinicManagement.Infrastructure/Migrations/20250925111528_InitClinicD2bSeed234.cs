using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClinicD2bSeed234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "RegistrationRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(9441));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(9443));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(9445));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(6358), new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(6352) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 611, DateTimeKind.Utc).AddTicks(6094));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 614, DateTimeKind.Utc).AddTicks(4844));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 614, DateTimeKind.Utc).AddTicks(4850));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 614, DateTimeKind.Utc).AddTicks(4851));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 25, 11, 15, 27, 614, DateTimeKind.Utc).AddTicks(4852));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "RegistrationRequests");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6689));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6692));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 20, 4, 32, 7, 72, DateTimeKind.Utc).AddTicks(6694));

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
    }
}
