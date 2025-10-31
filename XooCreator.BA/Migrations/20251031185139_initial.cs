using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
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
                    Summary = table.Column<string>(type: "text", nullable: true),
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
                    { new Guid("11111111-1111-1111-1111-111111111111"), "test-user-sub", new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6720), "test@example.com", false, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6720), "Test User", null, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6721) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "marian-test-sub", new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6725), "marian@example.com", false, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6726), "Marian", null, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6726) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "alchimalia-admin-sub", new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6729), "alchimalia@admin.com", false, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6730), "Marian Teacher", null, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6731) }
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
                    { new Guid("b20586da-db31-476a-a7f0-951c61396029"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2252), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2252) },
                    { new Guid("ee7a198e-8f23-46aa-ab95-9d16728f4a3c"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2227), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2227) },
                    { new Guid("f47fa6db-5e1c-44f9-8ab3-d297e68a6e05"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2185), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(2186) }
                });

            migrationBuilder.InsertData(
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3379), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3380) },
                    { "linkaro", 0, new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3375), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3375) },
                    { "puf-puf", 0, new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3359), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 10, 31, 18, 51, 38, 748, DateTimeKind.Utc).AddTicks(3361) }
                });

            migrationBuilder.InsertData(
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("05f2df49-d1d2-4023-b874-392b66073081"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9958), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9958) },
                    { new Guid("05fa9614-a0c4-4192-8130-d1be436243a0"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9874), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9875) },
                    { new Guid("143b3d78-eab3-44e3-941f-be8217bb6e66"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9955), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9955) },
                    { new Guid("2068a6a7-b1d4-4a31-afdf-aecdd30efc92"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9968), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9969) },
                    { new Guid("25c04900-8eef-41a8-b50d-42c61c45aba3"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9863), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9863) },
                    { new Guid("2cec14a5-18f9-445e-a437-be00d855f4cf"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9899), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9899) },
                    { new Guid("2e3d4e6f-f442-4b23-ada7-717e34a42c02"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9871), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9871) },
                    { new Guid("3495c1f7-67c8-4fd2-baa5-73cb93804520"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9923), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9923) },
                    { new Guid("36d92b8e-a0ff-4630-b59c-4b199e11427c"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9902), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9903) },
                    { new Guid("3fa0f8b2-78ef-43db-b4c2-98f4b35312c2"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9781), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9781) },
                    { new Guid("4296bbfb-3fba-4e42-ba42-abbae5ca4fe8"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9926), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9927) },
                    { new Guid("4e2f97e1-09f3-4458-a562-e75827115144"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9930), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9930) },
                    { new Guid("51eacf75-2a99-425c-9cb0-1fd9e99ad79b"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9909), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9909) },
                    { new Guid("5778a9d3-0b8f-404b-a14f-77ff868e0bed"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9961), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9962) },
                    { new Guid("63316874-cb91-4d05-83f3-a69860827fe9"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9917), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9917) },
                    { new Guid("7508121c-ee9e-4ad1-bd8f-0ad963ff75b8"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9760), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9760) },
                    { new Guid("89850ed2-1ff8-4e83-bbca-c7e0b5f8cd2d"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9892), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9893) },
                    { new Guid("8dfbb391-097a-40bc-b937-57c5ca0d61ee"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9879), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9879) },
                    { new Guid("8f573300-6dd4-4f93-857e-84468b49be25"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9937), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9938) },
                    { new Guid("8f717c31-03fb-4527-a263-4f351ef8930b"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9947), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9948) },
                    { new Guid("8f93012c-9658-4682-b2d4-747d5f4a3138"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9965), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9965) },
                    { new Guid("a9cd4ae6-eb66-4876-9504-cc8c87858bc9"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9741), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9741) },
                    { new Guid("ad6a6a57-8be5-4f65-a7e5-41ba15e032ab"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9906), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9906) },
                    { new Guid("af049981-6ed2-4307-847e-750b11b164e6"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9784), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9785) },
                    { new Guid("b3995179-6f45-4a41-b295-0f861e778297"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9979), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9980) },
                    { new Guid("b665aaff-a518-490b-be9a-5ce021d7338e"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9941), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9941) },
                    { new Guid("baa07960-844f-4c55-ae5c-a96ee6f0e5e1"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9944), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9945) },
                    { new Guid("bb0559e1-7f33-42cc-9c31-7f7deaf595bf"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9976), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9977) },
                    { new Guid("bb08f26f-c0e4-4b13-8e33-dd9cbbf17b96"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9913), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9914) },
                    { new Guid("c7965147-0381-4aa8-8860-1b8e32ea9bd8"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9971), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9972) },
                    { new Guid("d555c0a8-9f0c-484b-a6fe-d153a56be65a"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9933), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9933) },
                    { new Guid("f207bf7a-f9e7-4725-9a7e-fa015375ca42"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9882), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9883) },
                    { new Guid("f6245c19-43f8-4ccd-8214-0a3a0bf94e3e"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9764), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9765) },
                    { new Guid("f678e9ca-6bbe-472f-ad6b-75ff62a2913e"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9895), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9896) },
                    { new Guid("fc147df8-7743-4b36-bb1f-ce57fe24e0f9"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9867), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(9868) }
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
                columns: new[] { "Id", "Category", "CoverImageUrl", "CreatedAt", "CreatedBy", "IsActive", "SortOrder", "Status", "StoryCategory", "StoryId", "Summary", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "terra", "images/tol/stories/pp-prietenul-pierdut/0.cover.png", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(5674), new Guid("33333333-3333-3333-3333-333333333333"), true, 1, 1, 1, "learn-to-read-s1", null, "Puf-Puf și prietenul pierdut", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(5675), new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.InsertData(
                table: "StoryHeroes",
                columns: new[] { "Id", "CreatedAt", "HeroId", "ImageUrl", "IsActive", "UnlockConditionJson", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(1844), "puf-puf", "images/tol/stories/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(1845) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(1965), "linkaro", "images/tol/stories/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(1966) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(2059), "grubot", "images/tol/stories/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 10, 31, 18, 51, 38, 745, DateTimeKind.Utc).AddTicks(2060) }
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
                    { new Guid("24fe1037-dc54-4a34-946a-45cf4d42d3d8"), "body", "en-us", "Body" },
                    { new Guid("47fb666e-b5c7-4011-96b4-f092abcf9ae4"), "horn", "en-us", "Horn" },
                    { new Guid("486c58cf-5a52-4d42-8825-fe78b61f5cb6"), "legs", "en-us", "Legs" },
                    { new Guid("7dae6f59-4867-4ac0-8e07-e8c9fc8d7c75"), "arms", "en-us", "Arms" },
                    { new Guid("8fb28922-5431-4f2c-9590-71a06d200cde"), "tail", "en-us", "Tail" },
                    { new Guid("9a855722-7e5c-4c8b-b9ef-46cade394ab1"), "horns", "en-us", "Horns" },
                    { new Guid("af1526ec-a547-4f7f-bfb3-88801374dcd5"), "head", "en-us", "Head" },
                    { new Guid("c68ac9ff-c510-4a75-ad52-1f90ea47d19b"), "wings", "en-us", "Wings" }
                });

            migrationBuilder.InsertData(
                table: "CreditTransactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "Reference", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), 15, new DateTime(2025, 10, 30, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6940), "test-purchase-marian", 0, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), -5, new DateTime(2025, 10, 31, 16, 51, 38, 758, DateTimeKind.Utc).AddTicks(6953), "test-generation", 1, new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 5, 0, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6864) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 5, 0, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6866) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 1000, 0, new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6868) }
                });

            migrationBuilder.InsertData(
                table: "HeroProgress",
                columns: new[] { "Id", "HeroId", "HeroType", "SourceStoryId", "UnlockedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6903), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 31, 18, 51, 38, 758, DateTimeKind.Utc).AddTicks(6907), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "StoryDefinitionTranslations",
                columns: new[] { "Id", "LanguageCode", "StoryDefinitionId", "Title" },
                values: new object[,]
                {
                    { new Guid("3d13ba12-6085-4b94-9d03-cdbcf52c6006"), "ro-ro", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf și prietenul pierdut" },
                    { new Guid("a8a7e22b-cbc7-44dd-811a-0d3fe800937c"), "en-us", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf and the Lost Friend" },
                    { new Guid("ed45169b-5889-4e83-909c-85930d9481ee"), "hu-hu", new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf és az elveszett barát" }
                });

            migrationBuilder.InsertData(
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("302bd076-f5cb-463d-bf8f-17555840a343"), new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(928), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 },
                    { new Guid("f4bf5349-a025-488e-8cf0-9578ecd1293f"), new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(808), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 },
                    { new Guid("f799a6be-cfd0-47af-aca7-bf6bc964bc9c"), new DateTime(2025, 10, 31, 18, 51, 38, 746, DateTimeKind.Utc).AddTicks(505), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 }
                });

            migrationBuilder.InsertData(
                table: "StoryTiles",
                columns: new[] { "Id", "AudioUrl", "Caption", "CreatedAt", "ImageUrl", "Question", "SortOrder", "StoryDefinitionId", "Text", "TileId", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("474eb590-7a34-4ea8-89c5-e2b300fc34cf"), null, "Prietenul pierdut", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6321), "images/tol/stories/pp-prietenul-pierdut/3.pierdut.png", null, 3, new Guid("44444444-4444-4444-4444-444444444444"), "„M-am pierdut”, spune iepurașul. „Nu plânge”, spune Puf-Puf. „Te voi duce pe potecă spre casă!”", "3", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6322) },
                    { new Guid("4b0bbfda-e884-460d-906b-6ba8f2aa2b21"), null, "Povestea prieteniei", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6342), "images/tol/stories/pp-prietenul-pierdut/6.final.png", null, 6, new Guid("44444444-4444-4444-4444-444444444444"), "„Mulțumesc, Puf-Puf!” „Prietenii se ajută!”, spune pisoiul. Ploaia se oprește. Pe cer, un curcubeu. P ca Puf-Puf, P ca prieten!", "6", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6342) },
                    { new Guid("82a3609f-4ec4-4175-8463-0a48c416a487"), null, "Un plânset mic", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6305), "images/tol/stories/pp-prietenul-pierdut/2.planset.png", null, 2, new Guid("44444444-4444-4444-4444-444444444444"), "Se aude: „Pfff… pfff…”. Puf-Puf se oprește. „Cine plânge?” Din tufiș apare un iepuraș mic.", "2", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6306) },
                    { new Guid("868df149-7978-443f-8f1f-5d84a02a979a"), null, "Prin ploaie și pădure", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6328), "images/tol/stories/pp-prietenul_pierdut/4.ploaie-padure.png", null, 4, new Guid("44444444-4444-4444-4444-444444444444"), "Merg împreună prin pădure. Ploaia picură pe pietre. Puf-Puf ține umbrela. Iepurașul sare prin bălți: „Plip-plop!”", "4", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6328) },
                    { new Guid("8f6b611c-c43e-4b7b-bad6-224bdd2bcb02"), null, "Poiana cu pană", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6331), "images/tol/stories/pp-prietenul-pierdut/5.poiana.png", null, 5, new Guid("44444444-4444-4444-4444-444444444444"), "Ajung într-o poiană plină de flori. Pe jos e o pană strălucitoare. „Poiana asta e aproape!”, spune iepurașul. Zâmbește cu putere.", "5", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6332) },
                    { new Guid("a6d6c13f-173c-43b1-ab62-25e28af3b0b6"), null, "Puf-Puf pleacă", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6098), "images/tol/stories/pp-prietenul-pierdut/1.pleaca.png", null, 1, new Guid("44444444-4444-4444-4444-444444444444"), "Puf-Puf e un pisoi mic și pufos. Plouă peste pădure. Poartă o pelerină portocalie. „Ce plăcut e parfumul ploii!”, spune Puf-Puf.", "1", "page", new DateTime(2025, 10, 31, 18, 51, 38, 749, DateTimeKind.Utc).AddTicks(6098) }
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
                    { new Guid("0074584f-29f6-4be8-98a2-33cd7d623915"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("060d8adf-1efd-4f8f-bb28-a54f846088ec"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("072a10a6-87d5-490a-970e-2d1a299c5d94"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("090b79c3-3f05-4a58-90cf-a434b0c2161c"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("0bed9781-7345-4b85-9427-6b9227a9babf"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("11e97253-6d10-42dd-8673-6a61624c23f5"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("1aa00b94-a96c-4850-8c68-754af3449185"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("2180a7f2-da4d-420e-ae14-c0c50190243a"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("2247fcfe-67f9-4058-a9b5-72b42ff63631"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("22fc8c4c-2682-49fa-9375-cbc9e5833a15"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("3561101d-c18a-4306-b9d8-0c46d28ca500"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("41527f46-89c2-4dbd-b797-d842d2106a0a"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("63bff1df-fbda-4417-a61b-95ddf2de4d74"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("64159a53-bd62-40c8-b44b-7956fd4b6ad0"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("654e0aa2-fd85-4d38-b6d6-8ee9a1ce42ae"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("69cfc946-6ae3-4d1e-9ec3-c66426bfe39a"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("6e5af2a6-e7f6-408a-851c-281cf76b6f01"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("7c15098e-8f4f-4d14-b570-bb3f209825cd"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("819536ed-280f-4134-a3e7-40bac436424b"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("903b34d7-bccc-424d-a1be-0349050dab8d"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("90c8f8bf-e2d3-4aa9-9733-e7e007d6ad83"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("952f58d9-6c8c-440d-9951-8d94342a105d"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("bcb31f68-c184-420d-a417-170bc7c99b0a"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("bf058994-3d00-4c16-8143-084a36eb3c6a"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("bf3e25b8-5282-40e2-80d8-4b44231e547e"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("cc86e4e9-17be-4165-b268-3662e8aa0c98"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("d4784854-773a-4174-8ae1-70705f9ed7a4"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("d5f8fc18-68c7-40e3-8202-06c7190e1fa9"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("e2a5526b-b97c-4aef-994b-4f76c64fb247"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("e3e3a4c3-4247-42d3-9482-1ef2837f9cb5"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("e6849fac-cc5f-4090-8e46-853da445179d"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("eca8543d-70c0-4099-bde6-522d2f9bf208"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" },
                    { new Guid("f28891c5-3b19-4a0c-9a59-207f37d3eea3"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("f98b218c-210a-4a59-b476-0573da489714"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" }
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
