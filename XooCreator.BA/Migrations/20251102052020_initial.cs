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
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
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
                    StoryTopic = table.Column<string>(type: "text", nullable: true),
                    StoryType = table.Column<int>(type: "integer", nullable: false),
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
                columns: new[] { "Id", "Auth0Id", "CreatedAt", "Email", "FirstName", "HasVisitedImaginationLaboratory", "LastLoginAt", "LastName", "Name", "Picture", "Role", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "alchimalia-admin-sub", new DateTime(2025, 11, 2, 5, 20, 19, 139, DateTimeKind.Utc).AddTicks(4842), "alchimalia@admin.com", "Marian", false, new DateTime(2025, 11, 2, 5, 20, 19, 139, DateTimeKind.Utc).AddTicks(4842), "Teacher", "Marian Teacher", null, 2, new DateTime(2025, 11, 2, 5, 20, 19, 139, DateTimeKind.Utc).AddTicks(4843) });

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
                    { new Guid("41b4a2c6-ec94-4ea5-8663-16a532b21bb5"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2179), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2179) },
                    { new Guid("a73f819d-feb3-4f86-9e13-c156f5670296"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2177), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2177) },
                    { new Guid("b0f6b089-b020-44a4-abc3-00fea8c1783e"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2170), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2170) }
                });

            migrationBuilder.InsertData(
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2362), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2363) },
                    { "linkaro", 0, new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2359), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2360) },
                    { "puf-puf", 0, new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2348), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 2, 5, 20, 19, 137, DateTimeKind.Utc).AddTicks(2350) }
                });

            migrationBuilder.InsertData(
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("07d6070d-e127-4619-b7b1-498cb8846062"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9303), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9303) },
                    { new Guid("08a2eeb6-da4f-4e7c-83ae-1d2af6b54417"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9323), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9323) },
                    { new Guid("09993ee9-e630-4b67-858d-a3f635060d3b"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9308), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9308) },
                    { new Guid("09bf63cc-70be-4e0c-839c-df52f58e0b03"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9316), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9316) },
                    { new Guid("0f523e55-8e54-4166-89cd-325c0a014677"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9256), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9257) },
                    { new Guid("1140f39a-ba39-47a2-a807-ff4d22cf4a3a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9330), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9331) },
                    { new Guid("1c9cd3f7-bfd9-4fdd-98df-adbf1c160fc1"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9248), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9249) },
                    { new Guid("1e243b7d-d026-4715-a8ee-c3b38e18c200"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9318), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9319) },
                    { new Guid("2149a667-d688-424a-b32c-6e2f7891b221"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9292), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9292) },
                    { new Guid("342de5c9-27da-464e-b274-55de0773dbd0"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9335), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9335) },
                    { new Guid("3b5f9913-a231-4804-91af-2becb799cc04"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9337), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9337) },
                    { new Guid("466f1ab4-317f-451b-ad58-82d1a056e38e"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9333), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9333) },
                    { new Guid("4bef8cb9-82a7-45b1-b9b9-4ba630ad2136"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9305), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9305) },
                    { new Guid("4fe0460b-b99f-4c28-9bf0-cbb15ef7bfc3"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9325), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9325) },
                    { new Guid("738ea1d6-15c9-4a3b-88ce-51448a4f62dd"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9288), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9289) },
                    { new Guid("75447ac6-bfa7-4da3-96db-9474da1ae38f"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9338), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9339) },
                    { new Guid("843c505d-c80c-4c04-9e63-a58979d5274d"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9290), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9290) },
                    { new Guid("956c969a-c887-45b1-aea3-235f4f205d3b"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9322), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9322) },
                    { new Guid("9abefd07-9977-4fa8-8b4f-d5efe5d4c3bc"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9250), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9250) },
                    { new Guid("a04172d9-cbb8-412b-8449-03a40ae2d21d"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9329), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9329) },
                    { new Guid("a5bdacdd-e388-4ff0-81c5-8ae39fbc5c43"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9302), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9302) },
                    { new Guid("a8358790-b08e-49ce-ba91-5a3e864eedf8"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9246), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9247) },
                    { new Guid("a9586f24-17cf-4139-b8db-51e1517fe9b5"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9311), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9312) },
                    { new Guid("bbca960b-387b-47a5-9d6a-26e6a0c4d51a"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9313), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9313) },
                    { new Guid("bda2dbb3-a938-43bb-91f1-7731e1c1ba31"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9306), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9307) },
                    { new Guid("cc799238-f918-43f6-b54a-fb39879e2977"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9298), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9298) },
                    { new Guid("cd578d27-5c32-4a83-bfcb-65c96dbdb691"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9332), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9332) },
                    { new Guid("cdcc4869-6cf0-48ca-a72d-23674c490a64"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9310), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9310) },
                    { new Guid("d424899d-fa15-4795-9745-55382ff6c1c7"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9320), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9321) },
                    { new Guid("dabbe56d-47c0-4d0d-bb02-c4bd9282becd"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9326), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9326) },
                    { new Guid("dc59820c-4adf-4855-a256-8047522644d5"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9287), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9287) },
                    { new Guid("de1bcbb0-bde9-4e10-a465-8d41f4d3057b"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9296), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9296) },
                    { new Guid("ecd987cd-fc63-4ba9-acd9-bb99e2cbcb2e"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9317), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9317) },
                    { new Guid("f02e399f-6b54-4530-8822-d88115c92484"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9244), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9244) },
                    { new Guid("f53672ec-7f66-411e-a206-b571b0ca290d"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9294), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(9294) }
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
                table: "StoryHeroes",
                columns: new[] { "Id", "CreatedAt", "HeroId", "ImageUrl", "IsActive", "UnlockConditionJson", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4442), "puf-puf", "images/tol/stories/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4443) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4462), "linkaro", "images/tol/stories/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4462) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4472), "grubot", "images/tol/stories/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(4473) }
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
                    { new Guid("6276b1e0-ddb2-4ac1-843e-ffece35f2602"), "head", "en-us", "Head" },
                    { new Guid("65418392-f35c-431b-a0d8-ed370bd19d14"), "tail", "en-us", "Tail" },
                    { new Guid("84aea5b0-a745-4188-a47c-0e8308ad5eb9"), "horn", "en-us", "Horn" },
                    { new Guid("912d5e0c-8eee-457d-b7ba-44863a9e162c"), "wings", "en-us", "Wings" },
                    { new Guid("d290f911-a745-48cd-82c0-8ec0ecf0141b"), "body", "en-us", "Body" },
                    { new Guid("d792d372-4b1d-439a-b0c3-dcfca2260af0"), "horns", "en-us", "Horns" },
                    { new Guid("f60dc7bf-e344-4a78-a88b-3c5f14d22a0f"), "arms", "en-us", "Arms" },
                    { new Guid("fc22aa1c-0cb0-40d5-a0d8-b7a476840788"), "legs", "en-us", "Legs" }
                });

            migrationBuilder.InsertData(
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), 1000, 0, new DateTime(2025, 11, 2, 5, 20, 19, 139, DateTimeKind.Utc).AddTicks(4891) });

            migrationBuilder.InsertData(
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("16b15b33-63f1-4d9f-ad95-66525eb8ebbf"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(6219), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 },
                    { new Guid("27c0e1f8-c342-4bff-8928-b01eea0a81a4"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(6242), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 },
                    { new Guid("9dca5ebb-e606-430a-a19d-230c0dd44a48"), new DateTime(2025, 11, 2, 5, 20, 19, 136, DateTimeKind.Utc).AddTicks(6193), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 }
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
                    { new Guid("1c35cb9e-3e1d-4737-8850-b17b04115b39"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("1e5124f3-9a43-47b8-a0dd-d9da63e7d4b6"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("1f1374d9-22c6-4fe6-b43a-7c734e63b3e7"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("1f2d253b-247d-4927-9087-884baea1ea9e"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("2f043a30-296b-4266-9343-f4dd9755062e"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("315fd160-b570-407e-8dd6-b3f7407383f4"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("346a2f99-4acf-4918-90b0-23b49e2e174e"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("36767342-1203-4f3b-80a8-b50ddbf45831"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("3bf7bdb4-fb99-41ae-8a92-f10224d5894a"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("3e8ca560-2212-45ff-828c-53e0aa6e3b9f"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("444250ec-eb3e-4f13-ba7f-2a81dd04a023"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("4e16b760-7c5d-417b-9ac6-bb6a816eeb7b"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("4fcf05cc-8927-4124-8a17-f6ba8f902e5d"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("528ad64c-97cd-4443-96da-2386d0f22b6a"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("56378f54-9474-46c5-bd34-2bcc4883e216"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("61514937-825d-4eac-a111-39149635a45b"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("70317040-7921-4da0-a4f0-ab4739798809"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("780fd954-9785-441c-8259-adfa5c9c87ca"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("8a3bca64-c66c-44ad-a365-4b15299c7fd9"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("96e33e3d-3f48-4a00-9597-d531514d6b72"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("97274984-00ad-43ef-a905-8ecdb111ff0d"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("99443733-8c58-4aba-bec4-e30ac8eb2652"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("9df5b755-2267-43dc-ac86-2e85f3bc70f3"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("9eff929c-eacb-4d25-954a-dc3a82e8673a"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("a3e4a319-b793-44b7-89f6-5ff49b9f0d37"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("c1372e1d-4c41-44a8-9000-8d1cf7b48005"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("c38dc452-a87f-4e58-aeef-44d5a71a45c7"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("cb76c23e-894b-4550-9f2a-6fb52e9bba28"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("cd9d76e2-36cf-45a0-8904-2f93aec3866d"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("cea69033-9950-4813-b844-d384e67b4e8a"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("d948d7ed-c341-4ef1-bafd-ed7d339c0f7b"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("dd2fdeb4-326e-4000-95a0-883bc8807a4a"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("e55c4a45-edfd-43ff-8fc2-097b3366ec12"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" },
                    { new Guid("e858b6d5-5106-4709-ac36-82d51bdaa424"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" }
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
