using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoloProjectManagement.UI.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialTasksDataForProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProjectTasks",
                columns: new[] { "Id", "CreatedAt", "Description", "DueDate", "Name", "Priority", "ProjectId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create tables for cart items, sessions, and guest checkout persistent data.", new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Design Database Schema for Shopping Cart", 2, new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 2, new DateTime(2026, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Utc), "Implement backend API endpoints to handle checkout sessions and webhooks.", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Integrate Stripe Payment Gateway", 2, new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 1, new DateTime(2026, 9, 29, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 9, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Achieve 85%+ test coverage on order placement and discount logic.", new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Write Unit Tests for Checkout Flow", 1, new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 0, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Initialize repository, configure linting, and verify iOS/Android emulators.", new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Set up Flutter Cross-Platform Project", 2, new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"), 0, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Request permissions and fetch raw step count data from device sensors.", new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Implement Core Motion / Google Fit Integration", 2, new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"), 0, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Design visual step rings, historical graphs, and settings pages.", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Create UI Mockups for Daily Dashboard", 1, new Guid("b2c3d4e5-f67a-8b9c-0d1e-2f3a4b5c6d7e"), 0, null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Document all environment variables, build steps, and secret dependencies.", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Audit Legacy Jenkins Pipeline", 1, new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"), 2, new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2026, 9, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Write YAML configurations for build-on-push and pull request validation.", new DateTime(2026, 9, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Configure GitHub Actions Workflows", 2, new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"), 2, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Configure actions/cache to reduce average NPM and NuGet build times by 40%.", new DateTime(2026, 10, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Implement Dependency Caching", 0, new Guid("c3d4e5f6-7a8b-9c0d-1e2f-3a4b5c6d7e8f"), 2, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "ProjectTasks",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));
        }
    }
}
