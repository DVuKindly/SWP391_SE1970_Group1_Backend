using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClinicDb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "RegistrationRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(3800));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(3802));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(3804));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(1167), new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(1160) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 472, DateTimeKind.Utc).AddTicks(1019));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(9001));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(9003));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(9005));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8909));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8912));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8913));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8915));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8950));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8957));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8958));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8959));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8960));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 7, 20, 9, 474, DateTimeKind.Utc).AddTicks(8961));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "RegistrationRequests");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(3756));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(3759));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(3796));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(1213), new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(1209) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 497, DateTimeKind.Utc).AddTicks(1054));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9042));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9044));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9046));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8951));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8954));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8956));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8957));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8992));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(8999));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9000));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9001));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9003));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 4, 4, 25, 47, 499, DateTimeKind.Utc).AddTicks(9004));
        }
    }
}
