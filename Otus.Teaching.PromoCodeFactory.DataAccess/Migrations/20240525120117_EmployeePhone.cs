using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EmployeePhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                column: "Phone",
                value: null);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("f766e2bf-340a-46ea-bff3-f1700b435895"),
                column: "Phone",
                value: null);

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("cb98e283-fcb0-4baa-8541-39ffe9a3fe38"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2024, 5, 25, 15, 1, 17, 275, DateTimeKind.Local).AddTicks(1707), new DateTime(2024, 6, 8, 15, 1, 17, 275, DateTimeKind.Local).AddTicks(1720) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("cb98e283-fcb0-4baa-8541-39ffe9a3fe38"),
                columns: new[] { "BeginDate", "EndDate" },
                values: new object[] { new DateTime(2024, 5, 25, 14, 59, 48, 947, DateTimeKind.Local).AddTicks(4326), new DateTime(2024, 6, 8, 14, 59, 48, 947, DateTimeKind.Local).AddTicks(4338) });
        }
    }
}
