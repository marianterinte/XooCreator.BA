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
            migrationBuilder.EnsureSchema(
                name: "alchimalia_schema");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "AlchimaliaUsers",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Auth0Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Roles = table.Column<int[]>(type: "integer[]", nullable: false),
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
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
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
                name: "ClassicAuthors",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassicAuthors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroClickMessages",
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
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
                name: "StoryAgeGroups",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgeGroupId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MaxAge = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAgeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroes",
                schema: "alchimalia_schema",
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
                name: "StoryTopics",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DimensionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreeConfigurations",
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditWallets",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    DiscoveryBalance = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditWallets", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CreditWallets_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroProgress",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroTreeProgress",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryFeedbackPreferences",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PreferenceType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryFeedbackPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryFeedbackPreferences_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryFeedbacks",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FeedbackText = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    WhatLiked = table.Column<string>(type: "jsonb", nullable: false),
                    WhatDisliked = table.Column<string>(type: "jsonb", nullable: false),
                    WhatCouldBeBetter = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryFeedbacks_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryReaders",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcquisitionSource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryReaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryReaders_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trees",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStoryReadProgress",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokenBalances",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBestiary",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBestiary_BestiaryItems_BestiaryItemId",
                        column: x => x.BestiaryItemId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BestiaryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodyPartTranslations",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    StoryTopic = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ClassicAuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    StoryType = table.Column<int>(type: "integer", nullable: false),
                    PriceInCredits = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCrafts_ClassicAuthors_ClassicAuthorId",
                        column: x => x.ClassicAuthorId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "ClassicAuthors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    StoryTopic = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    ClassicAuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    StoryType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    PriceInCredits = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitions", x => x.Id);
                    table.UniqueConstraint("AK_StoryDefinitions_StoryId", x => x.StoryId);
                    table.ForeignKey(
                        name: "FK_StoryDefinitions_ClassicAuthors_ClassicAuthorId",
                        column: x => x.ClassicAuthorId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "ClassicAuthors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionTranslations",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAgeGroupTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryAgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAgeGroupTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryAgeGroupTranslations_StoryAgeGroups_StoryAgeGroupId",
                        column: x => x.StoryAgeGroupId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroUnlocks",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryHeroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTopicTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTopicTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryTopicTranslations_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryProgress",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeProgress",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeProgress_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeRegions",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeUnlockRules",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Creatures",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Creatures_Trees_TreeId",
                        column: x => x.TreeId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "Trees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TreeChoices",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "Trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAgeGroups",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryAgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftAgeGroups", x => new { x.StoryCraftId, x.StoryAgeGroupId });
                    table.ForeignKey(
                        name: "FK_StoryCraftAgeGroups_StoryAgeGroups_StoryAgeGroupId",
                        column: x => x.StoryAgeGroupId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryCraftAgeGroups_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTiles",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    TileId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftTiles_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTopics",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftTopics", x => new { x.StoryCraftId, x.StoryTopicId });
                    table.ForeignKey(
                        name: "FK_StoryCraftTopics_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryCraftTopics_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftTranslations_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionAgeGroups",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryAgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitionAgeGroups", x => new { x.StoryDefinitionId, x.StoryAgeGroupId });
                    table.ForeignKey(
                        name: "FK_StoryDefinitionAgeGroups_StoryAgeGroups_StoryAgeGroupId",
                        column: x => x.StoryAgeGroupId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryDefinitionAgeGroups_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionTopics",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitionTopics", x => new { x.StoryDefinitionId, x.StoryTopicId });
                    table.ForeignKey(
                        name: "FK_StoryDefinitionTopics_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryDefinitionTopics_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionTranslations",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPublicationAudits",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPublicationAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPublicationAudits_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPurchases",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    CreditsSpent = table.Column<double>(type: "double precision", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryPurchases_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryPurchases_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryReviews",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryReviews_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryReviews_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTiles",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCreatedStories",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCreatedStories_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteStories",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteStories_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteStories_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOwnedStories",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOwnedStories_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalPartSupports",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalPartSupports_BodyParts_PartKey",
                        column: x => x.PartKey,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalTranslations",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeStoryNodes",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeStoryNodes_TreeConfigurations_TreeConfigurationId",
                        column: x => x.TreeConfigurationId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreeStoryNodes_TreeRegions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAnswers",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftTileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerId = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftAnswers_StoryCraftTiles_StoryCraftTileId",
                        column: x => x.StoryCraftTileId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCraftTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTileTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftTileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Question = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftTileTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftTileTranslations_StoryCraftTiles_StoryCraftTileId",
                        column: x => x.StoryCraftTileId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCraftTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswers",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryTileTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryTileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Question = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTileTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryTileTranslations_StoryTiles_StoryTileId",
                        column: x => x.StoryTileId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAnswerTokens",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftAnswerTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftAnswerTokens_StoryCraftAnswers_StoryCraftAnswerId",
                        column: x => x.StoryCraftAnswerId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCraftAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAnswerTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryCraftAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftAnswerTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryCraftAnswerTranslations_StoryCraftAnswers_StoryCraftAn~",
                        column: x => x.StoryCraftAnswerId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCraftAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswerTokens",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryAnswerTranslations",
                schema: "alchimalia_schema",
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
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                columns: new[] { "Id", "Auth0Id", "CreatedAt", "Email", "FirstName", "HasVisitedImaginationLaboratory", "LastLoginAt", "LastName", "Name", "Picture", "Role", "Roles", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "alchimalia-admin-sub", new DateTime(2025, 11, 26, 16, 3, 31, 770, DateTimeKind.Utc).AddTicks(3433), "alchimalia@admin.com", "Marian", false, new DateTime(2025, 11, 26, 16, 3, 31, 770, DateTimeKind.Utc).AddTicks(3435), "Teacher", "Marian Teacher", null, 2, new[] { 2 }, new DateTime(2025, 11, 26, 16, 3, 31, 770, DateTimeKind.Utc).AddTicks(3436) });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
                table: "BuilderConfigs",
                columns: new[] { "Id", "BaseUnlockedAnimalIds", "BaseUnlockedBodyPartKeys" },
                values: new object[] { 1, "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]", "[\"head\",\"body\",\"arms\"]" });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2affdc01-8e18-48ff-811b-48f6a3df42c7"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2669), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2669) },
                    { new Guid("75b47910-07c3-4809-8f89-90790c049388"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2664), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2664) },
                    { new Guid("95818103-cda3-4096-bdbe-42d74993c25c"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2671), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2671) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2855), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2855) },
                    { "linkaro", 0, new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2852), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2853) },
                    { "puf-puf", 0, new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2847), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 16, 3, 31, 766, DateTimeKind.Utc).AddTicks(2848) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("023e5adb-771f-4f90-8a48-b25fb9f07760"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9427), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9428) },
                    { new Guid("0b5542bf-093f-4d11-9c67-61b779b520eb"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9400), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9400) },
                    { new Guid("0ef52232-7bc6-4144-a75c-ec74d0359d7b"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9396), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9396) },
                    { new Guid("1169249a-1ee1-49d9-a8b0-a69b04bc2b1d"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9432), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9432) },
                    { new Guid("140b0635-c924-45f0-8de1-950cbae1cdc2"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9421), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9422) },
                    { new Guid("234cabf4-6274-412c-8b89-28e24e7b5989"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9423), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9423) },
                    { new Guid("245e867d-451a-4c91-b7bd-946424cd504a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9437), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9438) },
                    { new Guid("2649b8de-68ad-44a7-a433-7357bbd7b291"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9435), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9435) },
                    { new Guid("288ee102-e284-4ae9-8aaf-208e2621310b"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9403), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9404) },
                    { new Guid("2bc365ee-cad8-4e19-ab0c-a6af856e66d9"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9436), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9436) },
                    { new Guid("39044d2a-9434-4dad-bca5-a23033b4dbb4"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9414), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9414) },
                    { new Guid("396eea45-ce31-426a-80f4-4e80968f097b"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9417), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9417) },
                    { new Guid("3d478c37-35de-4917-9485-0f13585a28ab"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9393), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9393) },
                    { new Guid("459190eb-6613-4fcf-b212-97e2cbe3b932"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9419), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9419) },
                    { new Guid("489512d1-b80a-4577-a5fc-8172d4985d30"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9413), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9413) },
                    { new Guid("4b02fa01-c610-4749-aa21-582f906ec294"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9347), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9347) },
                    { new Guid("4dd3c740-0ac0-49b8-96b2-028d1ec817c4"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9430), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9431) },
                    { new Guid("579802e1-59d1-490b-8987-b8ce2925bbfb"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9410), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9411) },
                    { new Guid("5acae1e6-0ce9-44d8-a0d1-23df27d9134a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9425), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9425) },
                    { new Guid("5c9937c6-2670-44a8-92ab-61449d1c4d8e"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9420), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9420) },
                    { new Guid("5f2c68d9-6fa5-4b52-9575-19704704c0ed"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9358), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9358) },
                    { new Guid("73684742-3614-4cac-b782-05d515b75401"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9352), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9352) },
                    { new Guid("74ff8c7c-4a6f-4c57-98a8-9aa9c730cc4c"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9433), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9433) },
                    { new Guid("78c1bc40-b90e-4e72-9271-ba639569e1cd"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9405), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9405) },
                    { new Guid("7b03f1f5-7a90-4999-8cbe-6c4a324bdb51"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9392), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9392) },
                    { new Guid("91a26097-427c-464a-aacd-d1bf8c5b82fb"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9406), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9406) },
                    { new Guid("944973b4-e52f-478c-ab6f-6098ae01f028"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9350), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9350) },
                    { new Guid("a22d77c4-023d-4fe1-9689-541f9d266d59"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9395), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9395) },
                    { new Guid("a504b6ac-2fdb-44c0-9027-b89b272bc051"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9398), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9399) },
                    { new Guid("ae5d3c15-20ea-4fae-b2ec-954e0af4e1f7"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9360), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9360) },
                    { new Guid("aed1beca-4b88-4bcb-a690-17afe9ae7235"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9441), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9441) },
                    { new Guid("cc3636d5-eee3-4dc8-8500-0c83afa101e5"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9408), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9408) },
                    { new Guid("efd9b01a-d248-441c-b255-57f54cbd9f1c"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9440), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9440) },
                    { new Guid("f13f6918-44c1-429d-891c-d089bb434020"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9409), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9409) },
                    { new Guid("f19ab8eb-9651-435a-86f2-0053b6f81fd9"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9426), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(9426) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                columns: new[] { "Id", "CreatedAt", "HeroId", "ImageUrl", "IsActive", "UnlockConditionJson", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3942), "puf-puf", "images/tol/stories/seed@alchimalia.com/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3942) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3962), "linkaro", "images/tol/stories/seed@alchimalia.com/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3962) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3971), "grubot", "images/tol/stories/seed@alchimalia.com/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(3971) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                columns: new[] { "Id", "BodyPartKey", "LanguageCode", "Name" },
                values: new object[,]
                {
                    { new Guid("2227721f-deaa-420b-bd76-f5d2bb152348"), "tail", "en-us", "Tail" },
                    { new Guid("3d315799-08cc-4cfe-91f1-02fd36505cad"), "head", "en-us", "Head" },
                    { new Guid("697d18a4-d183-4348-a0a1-434cf1e1de02"), "arms", "en-us", "Arms" },
                    { new Guid("7fe7e4cc-87ef-4920-9617-b87c660c3cd5"), "horns", "en-us", "Horns" },
                    { new Guid("8294e590-660a-4d10-b059-1144f3d2408c"), "body", "en-us", "Body" },
                    { new Guid("d9a9835a-f089-46c9-b3d3-5d5402526728"), "wings", "en-us", "Wings" },
                    { new Guid("de27778d-fe79-4ac6-816a-ad4337164df3"), "legs", "en-us", "Legs" },
                    { new Guid("fc2899b9-2677-4d82-9c26-a4f99cb371cd"), "horn", "en-us", "Horn" }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), 1000.0, 0.0, new DateTime(2025, 11, 26, 16, 3, 31, 770, DateTimeKind.Utc).AddTicks(3496) });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("021192f4-0fd5-4a0d-ae70-a7e8e9335ce6"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(5871), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 },
                    { new Guid("0f49066b-bd20-42fb-8040-7c6544c6c335"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(5826), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 },
                    { new Guid("9979fe87-cef7-45f7-8cc7-f9f7980d5dfb"), new DateTime(2025, 11, 26, 16, 3, 31, 765, DateTimeKind.Utc).AddTicks(5850), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
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
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                columns: new[] { "Id", "AnimalId", "Label", "LanguageCode" },
                values: new object[,]
                {
                    { new Guid("05640322-d6ab-4234-b4a7-636009aa7f6c"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("1a1e3d7d-8dfa-48f0-a2d0-35f55f7515df"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("27c3a28c-5a8b-4280-9974-3ee42542a580"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("2cfac062-1074-4657-ad7d-2a04aa0d6b6e"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("3331feef-ea00-40e4-9a73-28f6e3deb71d"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("4076c1ef-5198-490f-9176-73f3ec41cf26"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("4a4440c2-380a-46f9-9288-bb763b5245b9"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("4c40bece-14d0-4e94-b257-1b55424ab305"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("6187594d-9b9b-4571-848e-68f6e42943fd"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("62537ec5-b2eb-44fc-bcd0-3a0197bc54fd"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("634bb002-8fb3-485d-ade3-ec3e5e6dc7d6"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("7393b0d2-81dd-4fe3-9e46-3c3e8189bf78"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("7b1af2b8-0c0f-4d5e-83c7-f394d691bc41"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("80031e7c-a535-4939-8426-9c2bb33260bf"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("8c493588-f022-4e11-8f80-9ff9a74dff60"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("9530e371-2fad-4ddf-824f-b2f0befb8abf"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("96664e89-3b30-43fd-8c7b-e5602ad86603"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("9f4f09a7-27a6-4922-97c2-978acb1f48e4"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("9fe8971b-83c5-4d56-8e1b-7bb053eb754a"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("ac4ed34a-a69c-4730-a5ed-ba857413d635"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("b0f66de4-8337-488d-a568-27ca30e35f9e"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("b78bc0bb-4468-45c2-87c0-e6b3b26f3b04"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("b97260ed-b6a6-441d-bdb3-59b3a32dd337"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("ba676dc0-154f-43c8-897c-c69b08e37263"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("bb59d3eb-1232-4fea-884b-96939b697087"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("c1f9fee2-655f-4fe6-98ec-0a9bd9e68e89"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("c34d13c2-b44f-40bd-9f14-3b65396bca63"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("c851a6e6-1ba9-47b9-ad5a-3064e7b52c55"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("c9f349f6-bc24-4c5a-a57b-de76cf54eed5"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("cb8639b0-63d1-41bc-8b29-10d532589680"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("e2c3cda5-61b0-4e68-acea-5431f65f9990"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("fd9d50ad-2cfc-427d-b177-a2f69445c633"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("fe8a5881-4f36-4994-812b-8dc36e595af7"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("ff99c8d6-68c6-41ae-a2c5-fff20940a449"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlchimaliaUsers_Auth0Id",
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                column: "Auth0Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPartSupports_PartKey",
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                column: "PartKey");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_RegionId",
                schema: "alchimalia_schema",
                table: "Animals",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalTranslations_AnimalId_LanguageCode",
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                columns: new[] { "AnimalId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyPartTranslations_BodyPartKey_LanguageCode",
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                columns: new[] { "BodyPartKey", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassicAuthors_AuthorId",
                schema: "alchimalia_schema",
                table: "ClassicAuthors",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassicAuthors_LanguageCode_SortOrder",
                schema: "alchimalia_schema",
                table: "ClassicAuthors",
                columns: new[] { "LanguageCode", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_TreeId",
                schema: "alchimalia_schema",
                table: "Creatures",
                column: "TreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_UserId",
                schema: "alchimalia_schema",
                table: "Creatures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_UserId",
                schema: "alchimalia_schema",
                table: "CreditTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroClickMessages_HeroId",
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitions_Id",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionTranslations_HeroDefinitionId_LanguageCode",
                schema: "alchimalia_schema",
                table: "HeroDefinitionTranslations",
                columns: new[] { "HeroDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroMessages_HeroId_RegionId",
                schema: "alchimalia_schema",
                table: "HeroMessages",
                columns: new[] { "HeroId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroProgress_UserId_HeroId_HeroType",
                schema: "alchimalia_schema",
                table: "HeroProgress",
                columns: new[] { "UserId", "HeroId", "HeroType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroTreeProgress_UserId_NodeId",
                schema: "alchimalia_schema",
                table: "HeroTreeProgress",
                columns: new[] { "UserId", "NodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserId",
                schema: "alchimalia_schema",
                table: "Jobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                schema: "alchimalia_schema",
                table: "Regions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAgeGroups_AgeGroupId",
                schema: "alchimalia_schema",
                table: "StoryAgeGroups",
                column: "AgeGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAgeGroupTranslations_StoryAgeGroupId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryAgeGroupTranslations",
                columns: new[] { "StoryAgeGroupId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswers_StoryTileId_AnswerId",
                schema: "alchimalia_schema",
                table: "StoryAnswers",
                columns: new[] { "StoryTileId", "AnswerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswerTokens_StoryAnswerId",
                schema: "alchimalia_schema",
                table: "StoryAnswerTokens",
                column: "StoryAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryAnswerTranslations_StoryAnswerId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryAnswerTranslations",
                columns: new[] { "StoryAnswerId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAgeGroups_StoryAgeGroupId",
                schema: "alchimalia_schema",
                table: "StoryCraftAgeGroups",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswers_StoryCraftTileId_AnswerId",
                schema: "alchimalia_schema",
                table: "StoryCraftAnswers",
                columns: new[] { "StoryCraftTileId", "AnswerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswerTokens_StoryCraftAnswerId",
                schema: "alchimalia_schema",
                table: "StoryCraftAnswerTokens",
                column: "StoryCraftAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswerTranslations_StoryCraftAnswerId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryCraftAnswerTranslations",
                columns: new[] { "StoryCraftAnswerId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCrafts_ClassicAuthorId",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                column: "ClassicAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCrafts_StoryId",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTiles_StoryCraftId_TileId",
                schema: "alchimalia_schema",
                table: "StoryCraftTiles",
                columns: new[] { "StoryCraftId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTileTranslations_StoryCraftTileId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryCraftTileTranslations",
                columns: new[] { "StoryCraftTileId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTopics_StoryTopicId",
                schema: "alchimalia_schema",
                table: "StoryCraftTopics",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTranslations_StoryCraftId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryCraftTranslations",
                columns: new[] { "StoryCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionAgeGroups_StoryAgeGroupId",
                schema: "alchimalia_schema",
                table: "StoryDefinitionAgeGroups",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitions_ClassicAuthorId",
                schema: "alchimalia_schema",
                table: "StoryDefinitions",
                column: "ClassicAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitions_StoryId",
                schema: "alchimalia_schema",
                table: "StoryDefinitions",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionTopics_StoryTopicId",
                schema: "alchimalia_schema",
                table: "StoryDefinitionTopics",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionTranslations_StoryDefinitionId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryDefinitionTranslations",
                columns: new[] { "StoryDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFeedbackPreferences_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryFeedbackPreferences",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFeedbacks_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryFeedbacks",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroes_HeroId",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroUnlocks_StoryHeroId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                columns: new[] { "StoryHeroId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "StoryProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_UserId_StoryId_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "StoryProgress",
                columns: new[] { "UserId", "StoryId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublicationAudits_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "StoryPublicationAudits",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublicationAudits_StoryId",
                schema: "alchimalia_schema",
                table: "StoryPublicationAudits",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPurchases_StoryId",
                schema: "alchimalia_schema",
                table: "StoryPurchases",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPurchases_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryPurchases",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryReaders_StoryId",
                schema: "alchimalia_schema",
                table: "StoryReaders",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReaders_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryReaders",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryReviews_StoryId",
                schema: "alchimalia_schema",
                table: "StoryReviews",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReviews_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryReviews",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTiles_StoryDefinitionId_TileId",
                schema: "alchimalia_schema",
                table: "StoryTiles",
                columns: new[] { "StoryDefinitionId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTileTranslations_StoryTileId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryTileTranslations",
                columns: new[] { "StoryTileId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTopics_TopicId",
                schema: "alchimalia_schema",
                table: "StoryTopics",
                column: "TopicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTopicTranslations_StoryTopicId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryTopicTranslations",
                columns: new[] { "StoryTopicId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeChoices_TreeId_Tier",
                schema: "alchimalia_schema",
                table: "TreeChoices",
                columns: new[] { "TreeId", "Tier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeProgress_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeProgress_UserId_RegionId_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeProgress",
                columns: new[] { "UserId", "RegionId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeRegions_Id_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeRegions",
                columns: new[] { "Id", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeRegions_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeRegions",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trees_UserId",
                schema: "alchimalia_schema",
                table: "Trees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_RegionId",
                schema: "alchimalia_schema",
                table: "TreeStoryNodes",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_StoryId_RegionId_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeStoryNodes",
                columns: new[] { "StoryId", "RegionId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeStoryNodes_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeStoryNodes",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeUnlockRules_TreeConfigurationId",
                schema: "alchimalia_schema",
                table: "TreeUnlockRules",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBestiary_BestiaryItemId",
                schema: "alchimalia_schema",
                table: "UserBestiary",
                column: "BestiaryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBestiary_UserId_BestiaryItemId_BestiaryType",
                schema: "alchimalia_schema",
                table: "UserBestiary",
                columns: new[] { "UserId", "BestiaryItemId", "BestiaryType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCreatedStories_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserCreatedStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCreatedStories_UserId_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserCreatedStories",
                columns: new[] { "UserId", "StoryDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteStories_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserFavoriteStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteStories_UserId_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserFavoriteStories",
                columns: new[] { "UserId", "StoryDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnedStories_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserOwnedStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOwnedStories_UserId_StoryDefinitionId",
                schema: "alchimalia_schema",
                table: "UserOwnedStories",
                columns: new[] { "UserId", "StoryDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryReadProgress_UserId_StoryId_TileId",
                schema: "alchimalia_schema",
                table: "UserStoryReadProgress",
                columns: new[] { "UserId", "StoryId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokenBalances_UserId_Type_Value",
                schema: "alchimalia_schema",
                table: "UserTokenBalances",
                columns: new[] { "UserId", "Type", "Value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalPartSupports",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "BodyPartTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "BuilderConfigs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "Creatures",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "CreditTransactions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "CreditWallets",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroClickMessages",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroMessages",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroTreeProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAgeGroupTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAnswerTokens",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAnswerTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftAgeGroups",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftAnswerTokens",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftAnswerTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftTileTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftTopics",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDefinitionAgeGroups",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDefinitionTopics",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryFeedbackPreferences",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryFeedbacks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryHeroUnlocks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryPublicationAudits",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryPurchases",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryReaders",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryReviews",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryTileTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryTopicTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeChoices",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeStoryNodes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeUnlockRules",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserBestiary",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserCreatedStories",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserFavoriteStories",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserOwnedStories",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserStoryReadProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserTokenBalances",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "Animals",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "BodyParts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAnswers",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftAnswers",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAgeGroups",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryHeroes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryTopics",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "Trees",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeRegions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "BestiaryItems",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryTiles",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftTiles",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AlchimaliaUsers",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeConfigurations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "ClassicAuthors",
                schema: "alchimalia_schema");
        }
    }
}
