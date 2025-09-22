using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Safety",
                table: "UserTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TokensCostSafety",
                table: "HeroTreeProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HeroDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RewardsJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 19, 5, 7, 385, DateTimeKind.Utc).AddTicks(9520));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 17, 5, 7, 385, DateTimeKind.Utc).AddTicks(9525));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 22, 19, 5, 7, 385, DateTimeKind.Utc).AddTicks(9499));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 22, 19, 5, 7, 385, DateTimeKind.Utc).AddTicks(9500));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 19, 5, 7, 385, DateTimeKind.Utc).AddTicks(9372));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 19, 5, 7, 385, DateTimeKind.Utc).AddTicks(9374));

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitions_Id",
                table: "HeroDefinitions",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "Safety",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "TokensCostSafety",
                table: "HeroTreeProgress");

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 20, 21, 35, 57, 327, DateTimeKind.Utc).AddTicks(5208));

            migrationBuilder.UpdateData(
                table: "CreditTransactions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 19, 35, 57, 327, DateTimeKind.Utc).AddTicks(5224));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 21, 21, 35, 57, 327, DateTimeKind.Utc).AddTicks(5189));

            migrationBuilder.UpdateData(
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: new DateTime(2025, 9, 21, 21, 35, 57, 327, DateTimeKind.Utc).AddTicks(5191));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 21, 35, 57, 327, DateTimeKind.Utc).AddTicks(5066));

            migrationBuilder.UpdateData(
                table: "UsersAlchimalia",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 21, 35, 57, 327, DateTimeKind.Utc).AddTicks(5068));
        }
    }
}
