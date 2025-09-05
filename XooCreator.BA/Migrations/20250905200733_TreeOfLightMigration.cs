using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class TreeOfLightMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeroProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "text", nullable: false),
                    HeroType = table.Column<string>(type: "text", nullable: false),
                    SourceStoryId = table.Column<string>(type: "text", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroTreeProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<string>(type: "text", nullable: false),
                    TokensCostCourage = table.Column<int>(type: "integer", nullable: false),
                    TokensCostCuriosity = table.Column<int>(type: "integer", nullable: false),
                    TokensCostThinking = table.Column<int>(type: "integer", nullable: false),
                    TokensCostCreativity = table.Column<int>(type: "integer", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroTreeProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroTreeProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "text", nullable: true),
                    RewardReceived = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<string>(type: "text", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Courage = table.Column<int>(type: "integer", nullable: false),
                    Curiosity = table.Column<int>(type: "integer", nullable: false),
                    Thinking = table.Column<int>(type: "integer", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroProgress_UserId_HeroId_HeroType",
                table: "HeroProgress",
                columns: new[] { "UserId", "HeroId", "HeroType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroTreeProgress_UserId_NodeId",
                table: "HeroTreeProgress",
                columns: new[] { "UserId", "NodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_UserId_StoryId",
                table: "StoryProgress",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeProgress_UserId_RegionId",
                table: "TreeProgress",
                columns: new[] { "UserId", "RegionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroProgress");

            migrationBuilder.DropTable(
                name: "HeroTreeProgress");

            migrationBuilder.DropTable(
                name: "StoryProgress");

            migrationBuilder.DropTable(
                name: "TreeProgress");

            migrationBuilder.DropTable(
                name: "UserTokens");
        }
    }
}
