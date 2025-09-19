using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class drop_variant_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BestiaryDiscovered_UserId_DiscoveryItemId_VariantIndex",
                table: "BestiaryDiscovered");

            migrationBuilder.DropColumn(
                name: "VariantIndex",
                table: "BestiaryDiscovered");

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 20, 1, 55, 145, DateTimeKind.Utc).AddTicks(3995));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 18, 1, 55, 145, DateTimeKind.Utc).AddTicks(4007));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 19, 20, 1, 55, 145, DateTimeKind.Utc).AddTicks(3947));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 19, 20, 1, 55, 145, DateTimeKind.Utc).AddTicks(3949));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 20, 1, 55, 145, DateTimeKind.Utc).AddTicks(3825));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 20, 1, 55, 145, DateTimeKind.Utc).AddTicks(3827));

            migrationBuilder.CreateIndex(
                name: "IX_BestiaryDiscovered_UserId_DiscoveryItemId",
                table: "BestiaryDiscovered",
                columns: new[] { "UserId", "DiscoveryItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BestiaryDiscovered_UserId_DiscoveryItemId",
                table: "BestiaryDiscovered");

            migrationBuilder.AddColumn<int>(
                name: "VariantIndex",
                table: "BestiaryDiscovered",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 17, 36, 2, 262, DateTimeKind.Utc).AddTicks(4258));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 15, 36, 2, 262, DateTimeKind.Utc).AddTicks(4264));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 19, 17, 36, 2, 262, DateTimeKind.Utc).AddTicks(4238));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 19, 17, 36, 2, 262, DateTimeKind.Utc).AddTicks(4240));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 17, 36, 2, 262, DateTimeKind.Utc).AddTicks(4131));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 17, 36, 2, 262, DateTimeKind.Utc).AddTicks(4132));

            migrationBuilder.CreateIndex(
                name: "IX_BestiaryDiscovered_UserId_DiscoveryItemId_VariantIndex",
                table: "BestiaryDiscovered",
                columns: new[] { "UserId", "DiscoveryItemId", "VariantIndex" },
                unique: true);
        }
    }
}
