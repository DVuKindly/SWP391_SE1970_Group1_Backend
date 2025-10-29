using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitClinicD2bSeed2341 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegistrationRequestId = table.Column<int>(type: "int", nullable: false),
                    PaymentTransactionId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_Invoices_RegistrationRequests_RegistrationRequestId",
                        column: x => x.RegistrationRequestId,
                        principalTable: "RegistrationRequests",
                        principalColumn: "RegistrationRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(6726));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(6729));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(6730));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(5288), new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(5287) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 345, DateTimeKind.Utc).AddTicks(5159));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2806));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2808));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2810));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2738));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2740));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2741));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2742));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2772));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2776));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2778));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2779));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2781));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 9, 25, 347, DateTimeKind.Utc).AddTicks(2782));

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentTransactionId",
                table: "Invoices",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RegistrationRequestId",
                table: "Invoices",
                column: "RegistrationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(3537));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(3541));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(3543));

            migrationBuilder.UpdateData(
                table: "EmployeeRoles",
                keyColumns: new[] { "EmployeeId", "RoleId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "AssignedAtUtc", "CreatedAtUtc" },
                values: new object[] { new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(1864), new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(1862) });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeUserId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 113, DateTimeKind.Utc).AddTicks(1685));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6433));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6436));

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "ExamId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6439));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6331));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6334));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6336));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6337));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6384));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6389));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6391));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6393));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6394));

            migrationBuilder.UpdateData(
                table: "WorkPatternTemplates",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 20, 12, 50, 13, 115, DateTimeKind.Utc).AddTicks(6396));
        }
    }
}
