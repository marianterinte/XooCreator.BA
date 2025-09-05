using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class AddTestUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Auth0Sub", "CreatedAt", "DisplayName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "test-user-sub", new DateTime(2025, 9, 5, 20, 9, 4, 831, DateTimeKind.Utc).AddTicks(6873), "Test User" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
