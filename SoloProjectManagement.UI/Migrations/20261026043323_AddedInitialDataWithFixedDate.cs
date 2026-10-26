using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoloProjectManagement.UI.Migrations
{
    /// <inheritdoc />
    public partial class AddedInitialDataWithFixedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "CreatedAt", "DueDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"),
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"),
                columns: new[] { "CreatedAt", "DueDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "CreatedAt", "DueDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 10, 16, 4, 14, 26, 803, DateTimeKind.Utc).AddTicks(9354), new DateTime(2026, 12, 26, 4, 14, 26, 803, DateTimeKind.Utc).AddTicks(8264), new DateTime(2026, 10, 24, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(163) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"),
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2026, 10, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1253), new DateTime(2027, 2, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1243) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"),
                columns: new[] { "CreatedAt", "DueDate", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 9, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1263), new DateTime(2026, 10, 31, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1261), new DateTime(2026, 10, 25, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1264) });
        }
    }
}
