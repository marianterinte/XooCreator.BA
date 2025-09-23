using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HeroTreeProgress",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "HeroTreeProgress",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(557));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 10, 34, 19, 580, DateTimeKind.Utc).AddTicks(565));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(476));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(478));

            migrationBuilder.UpdateData(
                table: "HeroProgress",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UnlockedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(533));

            migrationBuilder.UpdateData(
                table: "HeroProgress",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UnlockedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(534));

            migrationBuilder.UpdateData(
                table: "UserTokens",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(504), new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(505) });

            migrationBuilder.UpdateData(
                table: "UserTokens",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(506), new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(507) });

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(333));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 12, 34, 19, 580, DateTimeKind.Utc).AddTicks(335));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9682));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 10, 28, 19, 344, DateTimeKind.Utc).AddTicks(9690));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9600));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9603));

            migrationBuilder.UpdateData(
                table: "HeroProgress",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UnlockedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "HeroProgress",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UnlockedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9663));

            migrationBuilder.InsertData(
                table: "HeroTreeProgress",
                columns: new[] { "Id", "NodeId", "TokensCostCourage", "TokensCostCreativity", "TokensCostCuriosity", "TokensCostSafety", "TokensCostThinking", "UnlockedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "seed", 0, 0, 0, 0, 0, new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9645), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "seed", 0, 0, 0, 0, 0, new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9647), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.UpdateData(
                table: "UserTokens",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9623), new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9624) });

            migrationBuilder.UpdateData(
                table: "UserTokens",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9626), new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9627) });

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9485));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 23, 12, 28, 19, 344, DateTimeKind.Utc).AddTicks(9486));
        }
    }
}
