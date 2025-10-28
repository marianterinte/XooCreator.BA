using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class FixIndependentStoriesTiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "AlchimaliaUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Auth0Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Picture = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasVisitedImaginationLaboratory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlchimaliaUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BestiaryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArmsKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BodyKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    HeadKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Story = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestiaryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BodyParts",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Image = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsBaseLocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyParts", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "BuilderConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseUnlockedAnimalIds = table.Column<string>(type: "text", nullable: false),
                    BaseUnlockedBodyPartKeys = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuilderConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroClickMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MessageKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroClickMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RewardsJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    PositionX = table.Column<double>(type: "numeric(10,6)", nullable: false),
                    PositionY = table.Column<double>(type: "numeric(10,6)", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MessageKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    StoryCategory = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitions", x => x.Id);
                    table.UniqueConstraint("AK_StoryDefinitions_StoryId", x => x.StoryId);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UnlockConditionJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryHeroes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreeConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditWallets",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<int>(type: "integer", nullable: false),
                    DiscoveryBalance = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditWallets", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CreditWallets_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_HeroProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
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
                    TokensCostSafety = table.Column<int>(type: "integer", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroTreeProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroTreeProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", maxLength: 24, nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", maxLength: 24, nullable: false),
                    ResultUrl = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RootType = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    CurrentTier = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trees_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStoryReadProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    TileId = table.Column<string>(type: "text", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoryReadProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStoryReadProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokenBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokenBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokenBalances_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBestiary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BestiaryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BestiaryType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DiscoveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBestiary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBestiary_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBestiary_BestiaryItems_BestiaryItemId",
                        column: x => x.BestiaryItemId,
                        principalTable: "BestiaryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodyPartTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartKey = table.Column<string>(type: "character varying(32)", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyPartTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodyPartTranslations_BodyParts_BodyPartKey",
                        column: x => x.BodyPartKey,
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Story = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionTranslations_HeroDefinitions_HeroDefinitionId",
                        column: x => x.HeroDefinitionId,
                        principalTable: "HeroDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Src = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsHybrid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryDefinitionTranslations_StoryDefinitions_StoryDefinitio~",
                        column: x => x.StoryDefinitionId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryMarketplaceInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    PriceInCredits = table.Column<int>(type: "integer", nullable: false),
                    Region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AgeRating = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Characters = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    EstimatedReadingTime = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryMarketplaceInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryMarketplaceInfos_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    CreditsSpent = table.Column<int>(type: "integer", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPurchases_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPurchases_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TileId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    Question = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryTiles_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCreatedStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreationNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreatedStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCreatedStories_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCreatedStories_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOwnedStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PurchaseReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOwnedStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOwnedStories_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOwnedStories_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroUnlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryHeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnlockOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryHeroUnlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryHeroUnlocks_StoryHeroes_StoryHeroId",
                        column: x => x.StoryHeroId,
                        principalTable: "StoryHeroes",
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
                    TokensJson = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TreeConfigurationId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
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
                    TreeConfigurationId = table.Column<string>(type: "text", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeRegions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PufpufMessage = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    TreeConfigurationId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeRegions_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeUnlockRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FromId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToRegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequiredStoriesCsv = table.Column<string>(type: "text", nullable: true),
                    MinCount = table.Column<int>(type: "integer", nullable: true),
                    StoryId = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    TreeConfigurationId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeUnlockRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeUnlockRules_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Creatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TreeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Archetype = table.Column<string>(type: "text", nullable: false),
                    TraitsJson = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ThumbUrl = table.Column<string>(type: "text", nullable: true),
                    PromptUsedJson = table.Column<string>(type: "text", nullable: false),
                    Story = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creatures_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Creatures_Trees_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Trees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TreeChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TreeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    BranchType = table.Column<string>(type: "text", nullable: false),
                    TraitAwarded = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeChoices_Trees_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalPartSupports",
                columns: table => new
                {
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartKey = table.Column<string>(type: "character varying(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalPartSupports", x => new { x.AnimalId, x.PartKey });
                    table.ForeignKey(
                        name: "FK_AnimalPartSupports_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalPartSupports_BodyParts_PartKey",
                        column: x => x.PartKey,
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalTranslations_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerId = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TokensJson = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryAnswers_StoryTiles_StoryTileId",
                        column: x => x.StoryTileId,
                        principalTable: "StoryTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTileTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Question = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTileTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryTileTranslations_StoryTiles_StoryTileId",
                        column: x => x.StoryTileId,
                        principalTable: "StoryTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeStoryNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RewardImageUrl = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    TreeConfigurationId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeStoryNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeStoryNodes_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeStoryNodes_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeStoryNodes_TreeRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "TreeRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswerTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAnswerTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryAnswerTokens_StoryAnswers_StoryAnswerId",
                        column: x => x.StoryAnswerId,
                        principalTable: "StoryAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswerTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAnswerTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryAnswerTranslations_StoryAnswers_StoryAnswerId",
                        column: x => x.StoryAnswerId,
                        principalTable: "StoryAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AlchimaliaUsers",
                columns: new[] { "Id", "Auth0Id", "CreatedAt", "Email", "HasVisitedImaginationLaboratory", "LastLoginAt", "Name", "Picture", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "test-user-sub", new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9259), "test@example.com", false, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9260), "Test User", null, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9260) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "marian-test-sub", new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9264), "marian@example.com", false, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9265), "Marian", null, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9265) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "alchimalia-admin-sub", new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9267), "alchimalia@admin.com", false, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9268), "Alchimalia-Admin", null, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9268) }
                });

            migrationBuilder.InsertData(
                table: "BodyParts",
                columns: new[] { "Key", "Image", "IsBaseLocked", "Name" },
                values: new object[,]
                {
                    { "arms", "images/bodyparts/hands.webp", false, "Arms" },
                    { "body", "images/bodyparts/body.webp", false, "Body" },
                    { "head", "images/bodyparts/face.webp", false, "Head" },
                    { "horn", "images/bodyparts/horn.webp", true, "Horn" },
                    { "horns", "images/bodyparts/horns.webp", true, "Horns" },
                    { "legs", "images/bodyparts/legs.webp", true, "Legs" },
                    { "tail", "images/bodyparts/tail.webp", true, "Tail" },
                    { "wings", "images/bodyparts/wings.webp", true, "Wings" }
                });

            migrationBuilder.InsertData(
                table: "BuilderConfigs",
                columns: new[] { "Id", "BaseUnlockedAnimalIds", "BaseUnlockedBodyPartKeys" },
                values: new object[] { 1, "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]", "[\"head\",\"body\",\"arms\"]" });

            migrationBuilder.InsertData(
                table: "HeroClickMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a9482f6c-e43b-41da-89ca-cbe942ba7ac4"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2877), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2877) },
                    { new Guid("c584577c-8a30-4f5b-a992-9dbd7c158d06"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2883), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2883) },
                    { new Guid("f774df1b-7521-48c1-8c83-08abbc59b918"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2881), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(2881) }
                });

            migrationBuilder.InsertData(
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3105), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3106) },
                    { "linkaro", 0, new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3102), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3102) },
                    { "puf-puf", 0, new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3096), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(3098) }
                });

            migrationBuilder.InsertData(
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("02962039-0d13-4012-b17c-90b498c311cb"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9360), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9360) },
                    { new Guid("0ac65359-b2ac-4216-a2a7-dacdadada90f"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9380), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9380) },
                    { new Guid("0bb723e4-9c24-4604-9284-2bb83b2f4c17"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9368), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9368) },
                    { new Guid("0c12041b-60d9-4d77-a930-ef18d1a35981"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9370), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9370) },
                    { new Guid("176181eb-0c72-41a0-9f24-784fb699f475"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9465), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9465) },
                    { new Guid("217767a9-9051-4092-bd3d-314c36119327"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9396), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9397) },
                    { new Guid("28484d88-1a6f-4aa2-9cd5-44795a1748d5"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9358), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9358) },
                    { new Guid("3369cab8-faa6-4379-965d-75b2d19380b6"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9384), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9384) },
                    { new Guid("3591c3be-dfb7-44ef-9262-d071f8c4e6c7"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9366), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9367) },
                    { new Guid("39ac8bb0-68e5-4ff8-b5b6-165aa7dfe479"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9381), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9382) },
                    { new Guid("3f1f8843-3448-4f86-86ec-0d179d7f6c4f"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9375), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9375) },
                    { new Guid("459f3b48-81db-4c8e-a429-71542ec298a5"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9404), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9404) },
                    { new Guid("47f827b2-0c3b-46d9-a713-9ce3b20b64ae"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9409), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9410) },
                    { new Guid("506f4022-6a5f-49d4-ba49-905ff8db7813"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9398), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9398) },
                    { new Guid("674c7e89-946e-4206-903f-907e51aaaa3f"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9391), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9391) },
                    { new Guid("69777c76-cb1c-45c4-8ea9-9a2d4846c332"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9378), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9379) },
                    { new Guid("75d670ec-b7c9-4b59-9414-0aa56d5e9f71"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9395), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9395) },
                    { new Guid("7672936e-d79b-4d4b-9491-6eae06727e9c"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9463), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9464) },
                    { new Guid("7a1a091e-7d5c-41e1-b947-d66e2dd30098"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9376), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9376) },
                    { new Guid("7cc25abb-da0b-469d-a8b1-6d705495bafd"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9392), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9392) },
                    { new Guid("7cd0f445-165d-4d53-a0d0-fa2f0174eb73"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9394), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9394) },
                    { new Guid("86f41c2d-4635-4d72-9eb3-ead65df677fd"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9405), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9405) },
                    { new Guid("9013c509-f3b1-4227-9fd4-7d3f0209e2aa"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9389), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9390) },
                    { new Guid("911b1bf3-1d90-42ba-a923-95009c154f46"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9386), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9386) },
                    { new Guid("a1303343-f2b2-4327-bc1d-5f14cc997e8a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9407), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9407) },
                    { new Guid("afc8eedb-73eb-48ae-bddb-7de304e5a3cb"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9383), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9383) },
                    { new Guid("b5b863fb-f9fe-48be-9045-94eec0d9851a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9408), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9408) },
                    { new Guid("b9c97c16-b09b-4c46-bca7-461b3070d5e0"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9361), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9362) },
                    { new Guid("c8e3d055-4b3d-4992-92d1-0f7c28e121a3"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9402), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9403) },
                    { new Guid("ceabad7b-07ca-4842-8269-9684e32a5e9b"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9346), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9347) },
                    { new Guid("d1772c0e-38bf-4bf6-b2de-8cae8bd30f95"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9371), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9371) },
                    { new Guid("dfde0295-f359-4eef-8147-8658c03e0440"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9411), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9411) },
                    { new Guid("ed309d83-7702-4cc8-89a8-d2ad686da8de"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9355), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9356) },
                    { new Guid("f1530443-5dcc-45da-bc6d-3146ff796764"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9412), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9413) },
                    { new Guid("fb47bf7a-2fcb-4ee5-8ece-0d2830043a2c"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9400), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(9400) }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Sahara" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Jungle" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Farm" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Savanna" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Forest" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "Wetlands" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "Mountains" }
                });

            migrationBuilder.InsertData(
                table: "StoryDefinitions",
                columns: new[] { "Id", "Category", "CoverImageUrl", "CreatedAt", "CreatedBy", "IsActive", "SortOrder", "Status", "StoryCategory", "StoryId", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "terra", "images/tol/stories/pp-prietenul-pierdut/0.cover.png", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8378), new Guid("33333333-3333-3333-3333-333333333333"), true, 1, 1, 1, "learn-to-read-s1", "Puf-Puf și prietenul pierdut", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8379), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.InsertData(
                table: "StoryHeroes",
                columns: new[] { "Id", "CreatedAt", "HeroId", "ImageUrl", "IsActive", "UnlockConditionJson", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(2926), "puf-puf", "images/tol/stories/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(2926) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(2998), "linkaro", "images/tol/stories/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(2998) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(3008), "grubot", "images/tol/stories/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(3009) }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Label", "RegionId", "Src" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/bunny.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", new Guid("10000000-0000-0000-0000-000000000006"), "images/animals/base/hippo.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/giraffe.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Dog", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/dog.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Fox", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/fox.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Cat", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/cat.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/monkey.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Camel", new Guid("10000000-0000-0000-0000-000000000001"), "images/animals/base/camel.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Deer", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/deer.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", new Guid("10000000-0000-0000-0000-000000000006"), "images/animals/base/duck.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/eagle.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/elephant.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/ostrich.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/parrot.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/jaguar.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/toucan.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/anaconda.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/capuchin_monkey.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/poison_dart_frog.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Lion", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/lion.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/african_elephant.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/giraffe.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/zebra.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/rhinoceros.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Bison", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/bison.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/saiga_antelope.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/gray_wolf.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/przewalski_horse.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/steppe_eagle.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/cow.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/sheep.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "Horse", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/horse.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/chicken.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Pig", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/pig.jpg" }
                });

            migrationBuilder.InsertData(
                table: "BodyPartTranslations",
                columns: new[] { "Id", "BodyPartKey", "LanguageCode", "Name" },
                values: new object[,]
                {
                    { new Guid("0e352a95-70fb-4946-8a3b-2225900fb3cd"), "horns", "en-us", "Horns" },
                    { new Guid("32f9c5b0-fe91-4ee6-899d-4312849286ad"), "body", "en-us", "Body" },
                    { new Guid("4921cb51-a96b-4c58-a39b-e50b6e83a536"), "legs", "en-us", "Legs" },
                    { new Guid("779a7113-7e51-4371-819d-5a9e0bb059c1"), "arms", "en-us", "Arms" },
                    { new Guid("7c4e0b51-996d-4bf7-bc94-1f40c87d0e3d"), "tail", "en-us", "Tail" },
                    { new Guid("9d11052f-1795-4988-bf0e-21bceecd0865"), "horn", "en-us", "Horn" },
                    { new Guid("a8d25463-2f36-4d03-9a2c-39c5f363813c"), "wings", "en-us", "Wings" },
                    { new Guid("a9931361-232d-4814-a219-5e76f856d35b"), "head", "en-us", "Head" }
                });

            migrationBuilder.InsertData(
                table: "CreditTransactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "Reference", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), 15, new DateTime(2025, 10, 27, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9361), "test-purchase-marian", 0, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), -5, new DateTime(2025, 10, 28, 19, 57, 26, 881, DateTimeKind.Utc).AddTicks(9367), "test-generation", 1, new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 5, 0, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9322) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 5, 0, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9323) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 1000, 0, new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9324) }
                });

            migrationBuilder.InsertData(
                table: "HeroProgress",
                columns: new[] { "Id", "HeroId", "HeroType", "SourceStoryId", "UnlockedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9342), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 28, 21, 57, 26, 881, DateTimeKind.Utc).AddTicks(9344), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "StoryDefinitionTranslations",
                columns: new[] { "Id", "LanguageCode", "StoryDefinitionId", "Title" },
                values: new object[,]
                {
                    { new Guid("10a6eae7-04d8-4368-895b-314f94ce701d"), "ro-ro", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf și prietenul pierdut" },
                    { new Guid("607a4d93-7b80-4ba6-84d1-832667b7512a"), "en-us", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf and the Lost Friend" },
                    { new Guid("775b34e2-891d-4f75-81ff-4fb2125f2731"), "hu-hu", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf és az elveszett barát" }
                });

            migrationBuilder.InsertData(
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("5c612bfc-c30f-4c20-9b9e-a81ef191e3bf"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(5313), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 },
                    { new Guid("68282117-1067-4708-92d5-7dc684ef4e5d"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(5362), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 },
                    { new Guid("dbae7a20-e01f-4f75-b1a0-80ff7540aa89"), new DateTime(2025, 10, 28, 21, 57, 26, 876, DateTimeKind.Utc).AddTicks(5340), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 }
                });

            migrationBuilder.InsertData(
                table: "StoryTiles",
                columns: new[] { "Id", "AudioUrl", "Caption", "CreatedAt", "ImageUrl", "Question", "SortOrder", "StoryDefinitionId", "Text", "TileId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("646fd2f1-d137-4a89-b814-0ec1a7dcbc1b"), null, "Poiana cu pană", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8831), "images/tol/stories/pp-prietenul-pierdut/5.poiana.png", null, 5, new Guid("44444444-4444-4444-4444-444444444444"), "Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.", "5", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8831) },
                    { new Guid("795a494d-1cbe-4d3e-b0f7-adeec2db3213"), null, "Povestea prieteniei", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8836), "images/tol/stories/pp-prietenul-pierdut/6.final.png", null, 6, new Guid("44444444-4444-4444-4444-444444444444"), "„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!", "6", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8837) },
                    { new Guid("94db2728-a2eb-4d14-ac21-a33da3d22ee2"), null, "Puf-Puf pleacă", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8769), "images/tol/stories/pp-prietenul-pierdut/1.pleaca.png", null, 1, new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.", "1", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8770) },
                    { new Guid("9cda9aa2-8d86-4630-bddc-315f1ef785b8"), null, "Prin ploaie și pădure", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8823), "images/tol/stories/pp-prietenul_pierdut/4.ploaie-padure.png", null, 4, new Guid("44444444-4444-4444-4444-444444444444"), "Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”", "4", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8823) },
                    { new Guid("a183ec54-c26f-411c-9413-d7b0bdd53365"), null, "Un plânset mic", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8818), "images/tol/stories/pp-prietenul-pierdut/2.planset.png", null, 2, new Guid("44444444-4444-4444-4444-444444444444"), "Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.", "2", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8818) },
                    { new Guid("da26705d-585e-4713-bc7f-bc9c10fc577f"), null, "Prietenul pierdut", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8821), "images/tol/stories/pp-prietenul-pierdut/3.pierdut.png", null, 3, new Guid("44444444-4444-4444-4444-444444444444"), "„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”", "3", "page", new DateTime(2025, 10, 28, 21, 57, 26, 877, DateTimeKind.Utc).AddTicks(8821) }
                });

            migrationBuilder.InsertData(
                table: "AnimalPartSupports",
                columns: new[] { "AnimalId", "PartKey" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000001"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000001"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "horns" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "horns" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "horns" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "head" }
                });

            migrationBuilder.InsertData(
                table: "AnimalTranslations",
                columns: new[] { "Id", "AnimalId", "Label", "LanguageCode" },
                values: new object[,]
                {
                    { new Guid("1903f172-871d-4b64-9a07-46331e7d6af6"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("20c526b2-1b28-4bc3-b636-f54c84f53fae"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("3d1befe4-aba1-4a10-ab47-f03480af4ba1"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("4e34e000-8639-4a47-bbfc-1ff723ee0578"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("60e34bbb-5e88-4831-a8fc-0a8b15fa70e2"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("68c2dd3b-c78e-48f7-8e18-c4bca603950e"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("6ce45b7a-2345-4beb-a82c-352848f9db78"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("72649d7f-a0ac-457d-9084-3b24601fd16c"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("78a32a2d-9aa9-4da4-8e43-d3b612a14ca4"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("7bb2f970-7edc-44fd-9ec1-3f16710d0716"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("7cd31f7d-92ec-4530-8ff4-47af87673d58"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" },
                    { new Guid("84e03d4b-3d16-428d-9d0c-6a39e80fbff3"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("8b900a60-1310-4172-a862-e3e293f8f7fc"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("8c263c62-e65e-46a2-9ad7-a457ac303e9d"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("8d49a746-cfbd-489f-860c-b33df5712e41"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("950d70f7-6e08-4862-a37b-9cb2a11acd06"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("9974b581-76b8-459e-98d0-4f1e51abf60b"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("9eeb0506-4346-4b68-9ef5-61550ee4ed7b"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("9fe945d4-2380-4ab0-806c-1a3ef4c21609"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("a5377d7d-991e-4029-a8bf-f3780840bdb7"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("af759cb0-9896-46f3-aa33-f2d34fdd9895"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("b03ba4ec-479d-4063-924a-18e66b8f2157"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("b0bbdd28-c34e-40b3-95e5-76fa7754f749"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("b34fcd47-bec8-413a-8c9e-9fbb12cedbb2"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("c1c05d75-1ec4-4c76-9922-bf3e5dab773f"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("ca26dcae-a772-45f2-85b1-11196fdcc85b"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("d78b40ec-1cdb-404a-8396-41eaa2774389"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("da0229b7-e31c-4f9f-bef5-80f2b357286a"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("dd38a9f4-5757-4e0e-bcd9-2be263265f64"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("e0934eb5-3905-4d0e-857a-9e70bcdc5681"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("e32b8ae1-2068-46cf-9c29-bdd6eed33e13"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("e3bff86b-c822-45e1-a853-7e379c48d4b9"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("ee61b79e-c975-4c9a-afef-f6e1967ac4af"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("f02b886d-490d-4974-8fe4-945439781d57"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlchimaliaUsers_Auth0Id",
                table: "AlchimaliaUsers",
                column: "Auth0Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPartSupports_PartKey",
                table: "AnimalPartSupports",
                column: "PartKey");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_RegionId",
                table: "Animals",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalTranslations_AnimalId_LanguageCode",
                table: "AnimalTranslations",
                columns: new[] { "AnimalId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyPartTranslations_BodyPartKey_LanguageCode",
                table: "BodyPartTranslations",
                columns: new[] { "BodyPartKey", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_TreeId",
                table: "Creatures",
                column: "TreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_UserId",
                table: "Creatures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_UserId",
                table: "CreditTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroClickMessages_HeroId",
                table: "HeroClickMessages",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitions_Id",
                table: "HeroDefinitions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionTranslations_HeroDefinitionId_LanguageCode",
                table: "HeroDefinitionTranslations",
                columns: new[] { "HeroDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroMessages_HeroId_RegionId",
                table: "HeroMessages",
                columns: new[] { "HeroId", "RegionId" },
                unique: true);

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
                name: "IX_Jobs_UserId",
                table: "Jobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswers_StoryTileId_AnswerId",
                table: "StoryAnswers",
                columns: new[] { "StoryTileId", "AnswerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswerTokens_StoryAnswerId",
                table: "StoryAnswerTokens",
                column: "StoryAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswerTranslations_StoryAnswerId_LanguageCode",
                table: "StoryAnswerTranslations",
                columns: new[] { "StoryAnswerId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitions_StoryId",
                table: "StoryDefinitions",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionTranslations_StoryDefinitionId_LanguageCode",
                table: "StoryDefinitionTranslations",
                columns: new[] { "StoryDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroes_HeroId",
                table: "StoryHeroes",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroUnlocks_StoryHeroId_StoryId",
                table: "StoryHeroUnlocks",
                columns: new[] { "StoryHeroId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryMarketplaceInfos_StoryId",
                table: "StoryMarketplaceInfos",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_TreeConfigurationId",
                table: "StoryProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_UserId_StoryId_TreeConfigurationId",
                table: "StoryProgress",
                columns: new[] { "UserId", "StoryId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPurchases_StoryId",
                table: "StoryPurchases",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPurchases_UserId_StoryId",
                table: "StoryPurchases",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTiles_StoryDefinitionId_TileId",
                table: "StoryTiles",
                columns: new[] { "StoryDefinitionId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTileTranslations_StoryTileId_LanguageCode",
                table: "StoryTileTranslations",
                columns: new[] { "StoryTileId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeChoices_TreeId_Tier",
                table: "TreeChoices",
                columns: new[] { "TreeId", "Tier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeProgress_TreeConfigurationId",
                table: "TreeProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeProgress_UserId_RegionId_TreeConfigurationId",
                table: "TreeProgress",
                columns: new[] { "UserId", "RegionId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeRegions_Id_TreeConfigurationId",
                table: "TreeRegions",
                columns: new[] { "Id", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeRegions_TreeConfigurationId",
                table: "TreeRegions",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trees_UserId",
                table: "Trees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_RegionId",
                table: "TreeStoryNodes",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_StoryId_RegionId_TreeConfigurationId",
                table: "TreeStoryNodes",
                columns: new[] { "StoryId", "RegionId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_TreeConfigurationId",
                table: "TreeStoryNodes",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeUnlockRules_TreeConfigurationId",
                table: "TreeUnlockRules",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBestiary_BestiaryItemId",
                table: "UserBestiary",
                column: "BestiaryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBestiary_UserId_BestiaryItemId_BestiaryType",
                table: "UserBestiary",
                columns: new[] { "UserId", "BestiaryItemId", "BestiaryType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCreatedStories_StoryDefinitionId",
                table: "UserCreatedStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCreatedStories_UserId_StoryDefinitionId",
                table: "UserCreatedStories",
                columns: new[] { "UserId", "StoryDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnedStories_StoryDefinitionId",
                table: "UserOwnedStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnedStories_UserId_StoryDefinitionId",
                table: "UserOwnedStories",
                columns: new[] { "UserId", "StoryDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryReadProgress_UserId_StoryId_TileId",
                table: "UserStoryReadProgress",
                columns: new[] { "UserId", "StoryId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokenBalances_UserId_Type_Value",
                table: "UserTokenBalances",
                columns: new[] { "UserId", "Type", "Value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalPartSupports");

            migrationBuilder.DropTable(
                name: "AnimalTranslations");

            migrationBuilder.DropTable(
                name: "BodyPartTranslations");

            migrationBuilder.DropTable(
                name: "BuilderConfigs");

            migrationBuilder.DropTable(
                name: "Creatures");

            migrationBuilder.DropTable(
                name: "CreditTransactions");

            migrationBuilder.DropTable(
                name: "CreditWallets");

            migrationBuilder.DropTable(
                name: "HeroClickMessages");

            migrationBuilder.DropTable(
                name: "HeroDefinitionTranslations");

            migrationBuilder.DropTable(
                name: "HeroMessages");

            migrationBuilder.DropTable(
                name: "HeroProgress");

            migrationBuilder.DropTable(
                name: "HeroTreeProgress");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "StoryAnswerTokens");

            migrationBuilder.DropTable(
                name: "StoryAnswerTranslations");

            migrationBuilder.DropTable(
                name: "StoryDefinitionTranslations");

            migrationBuilder.DropTable(
                name: "StoryHeroUnlocks");

            migrationBuilder.DropTable(
                name: "StoryMarketplaceInfos");

            migrationBuilder.DropTable(
                name: "StoryProgress");

            migrationBuilder.DropTable(
                name: "StoryPurchases");

            migrationBuilder.DropTable(
                name: "StoryTileTranslations");

            migrationBuilder.DropTable(
                name: "TreeChoices");

            migrationBuilder.DropTable(
                name: "TreeProgress");

            migrationBuilder.DropTable(
                name: "TreeStoryNodes");

            migrationBuilder.DropTable(
                name: "TreeUnlockRules");

            migrationBuilder.DropTable(
                name: "UserBestiary");

            migrationBuilder.DropTable(
                name: "UserCreatedStories");

            migrationBuilder.DropTable(
                name: "UserOwnedStories");

            migrationBuilder.DropTable(
                name: "UserStoryReadProgress");

            migrationBuilder.DropTable(
                name: "UserTokenBalances");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "BodyParts");

            migrationBuilder.DropTable(
                name: "HeroDefinitions");

            migrationBuilder.DropTable(
                name: "StoryAnswers");

            migrationBuilder.DropTable(
                name: "StoryHeroes");

            migrationBuilder.DropTable(
                name: "Trees");

            migrationBuilder.DropTable(
                name: "TreeRegions");

            migrationBuilder.DropTable(
                name: "BestiaryItems");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "StoryTiles");

            migrationBuilder.DropTable(
                name: "AlchimaliaUsers");

            migrationBuilder.DropTable(
                name: "TreeConfigurations");

            migrationBuilder.DropTable(
                name: "StoryDefinitions");
        }
    }
}
