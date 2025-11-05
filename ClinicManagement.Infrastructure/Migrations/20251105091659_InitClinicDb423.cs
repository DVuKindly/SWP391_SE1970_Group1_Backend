using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClinicDb423 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Employees_StaffId",
                table: "Prescriptions");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "Prescriptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(4650));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(4652));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(4654));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(1977), new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(1974) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 24, DateTimeKind.Utc).AddTicks(1767));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 27, DateTimeKind.Utc).AddTicks(16));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 27, DateTimeKind.Utc).AddTicks(18));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 27, DateTimeKind.Utc).AddTicks(20));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9922));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9925));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9926));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9928));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9967));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9974));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9975));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9977));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9978));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 9, 16, 59, 26, DateTimeKind.Utc).AddTicks(9979));

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Employees_StaffId",
                table: "Prescriptions",
                column: "StaffId",
                principalTable: "Employees",
                principalColumn: "EmployeeUserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Employees_StaffId",
                table: "Prescriptions");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "Prescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Employees_StaffId",
                table: "Prescriptions",
                column: "StaffId",
                principalTable: "Employees",
                principalColumn: "EmployeeUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
