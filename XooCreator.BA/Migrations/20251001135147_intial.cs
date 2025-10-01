using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

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
                name: "DiscoveryItems",
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
                    table.PrimaryKey("PK_DiscoveryItems", x => x.Id);
                });

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
                    PositionX = table.Column<double>(type: "numeric(10,6)", nullable: false),
                    PositionY = table.Column<double>(type: "numeric(10,6)", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitions", x => x.Id);
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
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitions", x => x.Id);
                    table.UniqueConstraint("AK_StoryDefinitions_StoryId", x => x.StoryId);
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
                name: "UsersAlchimalia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Auth0Sub = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAlchimalia", x => x.Id);
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
                name: "TreeRegions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PufpufMessage = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
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
                name: "BestiaryDiscovered",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscoveryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscoveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestiaryDiscovered", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BestiaryDiscovered_DiscoveryItems_DiscoveryItemId",
                        column: x => x.DiscoveryItemId,
                        principalTable: "DiscoveryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BestiaryDiscovered_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_CreditTransactions_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_CreditWallets_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_HeroProgress_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_HeroTreeProgress_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_Jobs_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_StoryProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryProgress_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_TreeProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeProgress_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_Trees_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_UserStoryReadProgress_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_UserTokenBalances_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
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
                        name: "FK_Creatures_Trees_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Trees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Creatures_UsersAlchimalia_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAlchimalia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "UsersAlchimalia",
                columns: new[] { "Id", "Auth0Sub", "CreatedAt", "DisplayName", "Email" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "test-user-sub", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(671), "Test User", "test@example.com" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "marian-test-sub", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(673), "Marian", "marian@example.com" }
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
                    { new Guid("2eadfb3f-eeb9-4906-b1db-1929cddeaf25"), "tail", "en-us", "Tail" },
                    { new Guid("4479a1fd-fad9-4bcb-8a81-2e761706c933"), "body", "en-us", "Body" },
                    { new Guid("82e63122-9075-4d33-8c24-02ff7a005a0c"), "horns", "en-us", "Horns" },
                    { new Guid("9f611029-2144-4d0f-a1da-6c9dccb963dd"), "horn", "en-us", "Horn" },
                    { new Guid("ba570f98-bda9-4705-bbc0-50c05f7d8ce2"), "wings", "en-us", "Wings" },
                    { new Guid("d13da71c-4960-4ccf-8c04-4953135f9bda"), "arms", "en-us", "Arms" },
                    { new Guid("d4d63c92-75e3-4a14-982f-1c71a7f456b0"), "head", "en-us", "Head" },
                    { new Guid("d821d599-db64-4f3b-b053-fcd5c759a248"), "legs", "en-us", "Legs" }
                });

            migrationBuilder.InsertData(
                table: "CreditTransactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "Reference", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), 15, new DateTime(2025, 9, 30, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(878), "test-purchase-marian", 0, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), -5, new DateTime(2025, 10, 1, 11, 51, 46, 240, DateTimeKind.Utc).AddTicks(890), "test-generation", 1, new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 5, 0, new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(738) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 10, 0, new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(739) }
                });

            migrationBuilder.InsertData(
                table: "HeroProgress",
                columns: new[] { "Id", "HeroId", "HeroType", "SourceStoryId", "UnlockedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(836), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "seed", "HERO_TREE_TRANSFORMATION", "", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(844), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "UserTokenBalances",
                columns: new[] { "Id", "CreatedAt", "Quantity", "Type", "UpdatedAt", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(766), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(767), new Guid("11111111-1111-1111-1111-111111111111"), "courage" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(774), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(775), new Guid("11111111-1111-1111-1111-111111111111"), "curiosity" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(781), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(781), new Guid("11111111-1111-1111-1111-111111111111"), "thinking" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(783), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(786), new Guid("11111111-1111-1111-1111-111111111111"), "creativity" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(788), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(788), new Guid("11111111-1111-1111-1111-111111111111"), "safety" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(790), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(790), new Guid("22222222-2222-2222-2222-222222222222"), "courage" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(792), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(792), new Guid("22222222-2222-2222-2222-222222222222"), "curiosity" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(794), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(794), new Guid("22222222-2222-2222-2222-222222222222"), "thinking" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(796), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(796), new Guid("22222222-2222-2222-2222-222222222222"), "creativity" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(799), 5, "TreeOfHeroes", new DateTime(2025, 10, 1, 13, 51, 46, 240, DateTimeKind.Utc).AddTicks(800), new Guid("22222222-2222-2222-2222-222222222222"), "safety" }
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
                    { new Guid("05dbac4f-b33b-4894-b767-f1909440f8be"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("246fc4ec-5df7-4767-9299-ab0569f08083"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("269feb15-80c6-4bc2-a878-67d44ec81f21"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("2bb11e23-562f-4c14-9890-6a976b4e158d"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("4903dca4-1774-4f24-acf6-9520b07df195"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" },
                    { new Guid("5b20c0e5-1a04-4579-85c6-52876d18e080"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("5bbc0206-e2f9-4670-bd45-5fe63e5e1ce8"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("62766bab-95b3-4e53-8465-11511c7580ae"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("632a0412-6d13-4c70-aca7-2c32055574d2"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("680f0181-89d5-4a7e-a0ca-d5aceaa520b6"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("73fc652e-18db-45db-b8d3-0dc673f48cb2"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("751dcbba-6e22-4263-80e0-8719ab0638e5"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("769cb22f-8346-490f-8285-9b2d18111a85"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("7c095a02-c05e-4f3b-a33f-c3f219f5c76a"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("7e224f6f-bef1-48c2-8460-c0aed1c9c852"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("8ad4a754-2dd1-4bdc-b9fe-1a5c9153ad6d"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("8ae36bfd-b92f-43c5-a959-d118df3da6da"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("8e8f154f-fe6b-41e3-b985-affd5cd9de16"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("90361729-1226-4db0-8d79-b19c9c4c91cf"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("9c5aac66-c99c-4246-a523-2c2ad5550e10"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("a29938f7-fcf9-48d4-95a2-8a15fbffc1f5"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("a5b95728-abe4-4e9c-b1a6-0e51abe786cc"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("a9ab0932-dd0e-4348-b827-6d6430bed869"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("b25f819b-b0c2-46aa-8b85-41a123042ff9"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("b89c5497-7302-4f08-8d8b-a2d4f6589030"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("c45662b8-038c-4e6b-bf9e-034c8b112901"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("c4e8c93d-4be8-4546-86ba-d2910eadb972"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("cbdac4a1-1bfb-4a30-a3cc-9b0114a0f22b"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("da8eeb55-2f9a-434d-a574-4997562ac811"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("dab49fc6-63be-42df-817e-f804f48297e0"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("e0966f94-daf5-4f52-8af6-198ab0e71a19"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("e790dd51-4a5a-43e0-b80c-488150024056"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("f21ef980-bc42-408b-8cae-506d47e7cfdb"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("f2b62f5b-95aa-4b6b-959e-98f834fdc19c"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" }
                });

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
                name: "IX_BestiaryDiscovered_DiscoveryItemId",
                table: "BestiaryDiscovered",
                column: "DiscoveryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BestiaryDiscovered_UserId_DiscoveryItemId",
                table: "BestiaryDiscovered",
                columns: new[] { "UserId", "DiscoveryItemId" },
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
                name: "IX_HeroDefinitions_Id",
                table: "HeroDefinitions",
                column: "Id",
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
                name: "IX_StoryProgress_TreeConfigurationId",
                table: "StoryProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_UserId_StoryId_TreeConfigurationId",
                table: "StoryProgress",
                columns: new[] { "UserId", "StoryId", "TreeConfigurationId" },
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
                name: "IX_UsersAlchimalia_Auth0Sub",
                table: "UsersAlchimalia",
                column: "Auth0Sub",
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
                name: "BestiaryDiscovered");

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
                name: "HeroDefinitions");

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
                name: "StoryProgress");

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
                name: "UserStoryReadProgress");

            migrationBuilder.DropTable(
                name: "UserTokenBalances");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "DiscoveryItems");

            migrationBuilder.DropTable(
                name: "BodyParts");

            migrationBuilder.DropTable(
                name: "StoryAnswers");

            migrationBuilder.DropTable(
                name: "Trees");

            migrationBuilder.DropTable(
                name: "TreeRegions");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "StoryTiles");

            migrationBuilder.DropTable(
                name: "UsersAlchimalia");

            migrationBuilder.DropTable(
                name: "TreeConfigurations");

            migrationBuilder.DropTable(
                name: "StoryDefinitions");
        }
    }
}
