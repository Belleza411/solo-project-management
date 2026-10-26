using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoloProjectManagement.UI.Migrations
{
    /// <inheritdoc />
    public partial class AddedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "DueDate", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 0, new DateTime(2026, 10, 16, 4, 14, 26, 803, DateTimeKind.Utc).AddTicks(9354), "Develop the core checkout flow and shopping cart service.", new DateTime(2026, 12, 26, 4, 14, 26, 803, DateTimeKind.Utc).AddTicks(8264), 1, "E-Commerce Platform", new DateTime(2026, 10, 24, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(163) },
                    { new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"), 1, new DateTime(2026, 10, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1253), "Build iOS and Android MVP for tracking user daily steps.", new DateTime(2027, 2, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1243), 0, "Mobile Fitness Tracker", null },
                    { new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"), 5, new DateTime(2026, 9, 26, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1263), "Migrate legacy build pipelines to GitHub Actions for faster builds.", new DateTime(2026, 10, 31, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1261), 2, "CI/CD Pipeline Optimization", new DateTime(2026, 10, 25, 4, 14, 26, 804, DateTimeKind.Utc).AddTicks(1264) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"));

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"));

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"));
        }
    }
}
