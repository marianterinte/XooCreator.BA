using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class authors : Migration
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
                name: "ClassicAuthors",
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
                name: "StoryAgeGroups",
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
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditWallets",
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
                name: "StoryFeedbackPreferences",
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
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryFeedbacks",
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
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryReaders",
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
                name: "StoryCrafts",
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
                        principalTable: "ClassicAuthors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalTable: "ClassicAuthors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "StoryAgeGroupTranslations",
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
                        principalTable: "StoryAgeGroups",
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
                name: "StoryTopicTranslations",
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
                        principalTable: "StoryTopics",
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
                name: "StoryCraftAgeGroups",
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
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryCraftAgeGroups_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTiles",
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
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTopics",
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
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryCraftTopics_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTranslations",
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
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionAgeGroups",
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
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryDefinitionAgeGroups_StoryDefinitions_StoryDefinitionId",
                        column: x => x.StoryDefinitionId,
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionTopics",
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
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryDefinitionTopics_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalTable: "StoryTopics",
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
                name: "StoryPublicationAudits",
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
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPurchases",
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
                name: "StoryReviews",
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
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryReviews_StoryDefinitions_StoryId",
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
                name: "UserFavoriteStories",
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
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteStories_StoryDefinitions_StoryDefinitionId",
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
                name: "StoryCraftAnswers",
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
                        principalTable: "StoryCraftTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftTileTranslations",
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
                        principalTable: "StoryCraftTiles",
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
                        principalTable: "StoryTiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAnswerTokens",
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
                        principalTable: "StoryCraftAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftAnswerTranslations",
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
                        principalTable: "StoryCraftAnswers",
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
                columns: new[] { "Id", "Auth0Id", "CreatedAt", "Email", "FirstName", "HasVisitedImaginationLaboratory", "LastLoginAt", "LastName", "Name", "Picture", "Role", "Roles", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "alchimalia-admin-sub", new DateTime(2025, 11, 22, 22, 32, 57, 320, DateTimeKind.Utc).AddTicks(1000), "alchimalia@admin.com", "Marian", false, new DateTime(2025, 11, 22, 22, 32, 57, 320, DateTimeKind.Utc).AddTicks(1001), "Teacher", "Marian Teacher", null, 2, new[] { 2 }, new DateTime(2025, 11, 22, 22, 32, 57, 320, DateTimeKind.Utc).AddTicks(1001) });

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
                    { new Guid("2a5ecc9d-0c56-4255-a124-5c262c56a941"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6202), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6203) },
                    { new Guid("926a549c-d86e-4e1b-a0a8-e3a30b51fb5a"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6211), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6211) },
                    { new Guid("9a360b01-7be7-4fdb-8549-737e281ab15e"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6213), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6213) }
                });

            migrationBuilder.InsertData(
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6909), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6909) },
                    { "linkaro", 0, new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6906), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6906) },
                    { "puf-puf", 0, new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6900), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(6902) }
                });

            migrationBuilder.InsertData(
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("082b2a0c-b41b-4b21-a4f2-5381b46b8916"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2126), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2126) },
                    { new Guid("0916b48e-da03-4a71-b523-b04ac391058b"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2173), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2173) },
                    { new Guid("100f213a-74be-4044-beb7-c69ce803c7b9"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2170), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2171) },
                    { new Guid("10212e05-fe1c-4dda-afd3-1223c76b96e0"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2141), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2142) },
                    { new Guid("19cc02d1-0820-412f-9a1d-d4a4ad247d59"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2159), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2160) },
                    { new Guid("1bed4c12-f7b3-40c8-9302-6e465c0371e1"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2098), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2099) },
                    { new Guid("1fb81b0d-da7e-45a1-a97f-57a6b61d8f66"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2151), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2151) },
                    { new Guid("21a3b45e-3db2-4428-a303-d97cd13df692"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2143), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2143) },
                    { new Guid("27bea25b-553c-4258-bec7-c66323a7eeba"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2158), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2158) },
                    { new Guid("33f99041-aa23-41aa-ac2c-f2b8f4551008"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2164), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2164) },
                    { new Guid("340583d8-c520-4ad3-918b-133da4bd6a69"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2168), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2168) },
                    { new Guid("355ffbc7-c019-4331-9d47-562d7ce1a71a"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2149), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2150) },
                    { new Guid("3dc4b9b8-0e62-4ca7-a6cd-84c78da6e1ad"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2175), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2176) },
                    { new Guid("454ce597-7624-4283-9d5f-d24750061ecf"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2086), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2086) },
                    { new Guid("4591f9a2-b6e9-48ec-a9e3-53fc7aafe28a"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2127), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2127) },
                    { new Guid("45e5bee5-e107-409c-9890-ca6114e33aa2"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2089), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2090) },
                    { new Guid("4c83b84c-4ab5-4b69-b55d-5f5d2348b69b"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2140), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2140) },
                    { new Guid("6841a0d4-fe55-49ae-979b-ee91e26ff7a5"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2134), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2134) },
                    { new Guid("7387138d-0e76-4ad8-831f-6cc7d1074f26"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2152), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2152) },
                    { new Guid("76135d15-6aa5-44d4-821f-38a48f92272d"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2144), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2144) },
                    { new Guid("7bf0890b-468d-4ed0-b217-4e97a01223ba"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2169), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2169) },
                    { new Guid("7f04f13d-4f8d-4699-8293-13f48ad18e9c"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2124), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2124) },
                    { new Guid("8032b8f2-6902-48e3-9ff3-1ccb35ce1cd2"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2155), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2155) },
                    { new Guid("80d82c74-0d69-4578-a2f7-6b2a42bc6a16"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2131), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2131) },
                    { new Guid("819e503a-4b6c-498b-93a7-a16bbfaaf0a0"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2136), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2137) },
                    { new Guid("82c63fa5-8c05-4904-bb99-b3e389f07168"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2162), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2162) },
                    { new Guid("8ab24048-f0f7-43fc-9f7d-53ae636cd432"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2156), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2157) },
                    { new Guid("99fccb22-1769-4d8b-94b7-4c915528ec3e"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2129), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2129) },
                    { new Guid("a2566a8b-875c-49bd-ae86-f5697b2452ba"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2165), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2165) },
                    { new Guid("adfaaa44-5102-4f41-a3da-8776d2b472ee"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2147), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2147) },
                    { new Guid("b296b2be-6470-42f7-afb0-bff82026129d"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2138), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2139) },
                    { new Guid("bc855ce1-675c-483d-b45a-c0e677bcf060"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2097), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2097) },
                    { new Guid("c800681d-e576-4300-9a4c-0fc96cc82522"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2166), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2167) },
                    { new Guid("da2f849a-5978-4f26-8553-7af0b461757a"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2153), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2154) },
                    { new Guid("e4d1ff9e-1ad9-4d55-9e3d-dfd8146c80c5"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2100), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 22, 22, 32, 57, 317, DateTimeKind.Utc).AddTicks(2100) }
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
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5169), "puf-puf", "images/tol/stories/seed@alchimalia.com/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5169) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5208), "linkaro", "images/tol/stories/seed@alchimalia.com/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5209) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5220), "grubot", "images/tol/stories/seed@alchimalia.com/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(5221) }
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
                    { new Guid("032abd9a-f8c7-4c7f-9ef3-dc85f845caa5"), "legs", "en-us", "Legs" },
                    { new Guid("077b54a4-f452-4a45-b3ac-c9c87e7f4155"), "body", "en-us", "Body" },
                    { new Guid("34e98f52-ffcb-40d6-b094-03d95243a5ad"), "head", "en-us", "Head" },
                    { new Guid("6a19407e-80ef-43cf-a818-2bcf2884e222"), "wings", "en-us", "Wings" },
                    { new Guid("86b62105-92c0-4992-ae3b-d5274c559a9f"), "tail", "en-us", "Tail" },
                    { new Guid("930f9d0f-de4e-4f52-abfd-7f884c40a369"), "arms", "en-us", "Arms" },
                    { new Guid("c878ab8c-4cb1-4f69-91aa-425dfe0f4823"), "horn", "en-us", "Horn" },
                    { new Guid("d73a9560-6136-47ce-bb4a-7829a15676c4"), "horns", "en-us", "Horns" }
                });

            migrationBuilder.InsertData(
                table: "CreditWallets",
                columns: new[] { "UserId", "Balance", "DiscoveryBalance", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), 1000.0, 0.0, new DateTime(2025, 11, 22, 22, 32, 57, 320, DateTimeKind.Utc).AddTicks(1092) });

            migrationBuilder.InsertData(
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("57b29403-d706-44f7-a139-fc4e2f838b17"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(7739), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 },
                    { new Guid("a461f6a5-0d37-4771-b0e0-b2739bc6924e"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(7717), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 },
                    { new Guid("fa6d4055-098c-4338-9cc4-4b0a8856f35f"), new DateTime(2025, 11, 22, 22, 32, 57, 316, DateTimeKind.Utc).AddTicks(7692), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 }
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
                    { new Guid("081043b7-8665-4996-88b0-8a6d34b88a9e"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("132d7d33-696c-4cb0-ab78-7fd1d252df9b"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("1f657394-32bf-4a71-bc0d-98ff33920a53"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("313555dc-e694-4672-a469-0540dd7e8411"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("36b8eec9-4d23-49c3-ba10-745372311478"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("4159151b-01ce-4a3d-88d9-d3a93fb514a3"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("443642d2-a439-4ad7-adde-dce70ff83a21"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("4beeae78-6460-4dfc-93e6-7914140fec41"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("5167f9e3-f888-4636-8faf-af57f90e62f4"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("52a66955-4667-4f82-ae51-295abeac218e"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("53d69b1c-3549-4483-ab91-83754974c45b"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("5575d12a-1a33-452e-805d-26c69039c049"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("646234fb-f0a9-4994-bcc3-9ef2ccd0386a"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("6983d998-82ed-4c22-92aa-b5f8923b5d7e"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("6ec42ca7-ac8f-4487-814b-1ee308b8414b"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("6f7cba16-56d2-4ae3-a028-29b249c4f0e2"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("71b25859-9791-42df-8df0-296c1f1209aa"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("758a5b3e-4eb4-4e00-a383-6cce634346ac"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("81d46542-354b-4a58-9f12-20bdc08f03b6"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("8cc4c306-3f66-4a99-adb1-d6fabcda6bca"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("93ec8847-f88d-49bd-91e5-864a4d2be0c0"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("a98e5c76-9091-41d8-b271-89a11b309de0"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("c6daf1db-1cf7-473d-9f94-15fe14590301"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("c85a73a4-991c-4158-a6d5-ea1ccc64d26c"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("ca5c040c-5d11-49e7-ab3f-02aa371c3477"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("d6b5ab28-c862-449d-b455-81a905d25d2c"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("db1d5e07-4be9-45c3-a2ee-98283ca25a27"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" },
                    { new Guid("dc284d18-dc84-44db-8a32-392e410852af"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("dcd8f15b-8885-4420-aef7-53abfbe13ee8"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("e7b4f93a-2899-4361-bd4e-82b13902ea31"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("ebf53217-385c-4d1d-b877-52ece1e49207"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("ee2a93e3-cd1a-4b13-b264-56945b0c3813"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("f457ab5c-5de4-405d-8612-3d65cf8095cb"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("fc130108-d201-4029-a994-36cc66216124"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" }
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
                name: "IX_ClassicAuthors_AuthorId",
                table: "ClassicAuthors",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassicAuthors_LanguageCode_SortOrder",
                table: "ClassicAuthors",
                columns: new[] { "LanguageCode", "SortOrder" });

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
                name: "IX_StoryAgeGroups_AgeGroupId",
                table: "StoryAgeGroups",
                column: "AgeGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAgeGroupTranslations_StoryAgeGroupId_LanguageCode",
                table: "StoryAgeGroupTranslations",
                columns: new[] { "StoryAgeGroupId", "LanguageCode" },
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
                name: "IX_StoryCraftAgeGroups_StoryAgeGroupId",
                table: "StoryCraftAgeGroups",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswers_StoryCraftTileId_AnswerId",
                table: "StoryCraftAnswers",
                columns: new[] { "StoryCraftTileId", "AnswerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswerTokens_StoryCraftAnswerId",
                table: "StoryCraftAnswerTokens",
                column: "StoryCraftAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftAnswerTranslations_StoryCraftAnswerId_LanguageCode",
                table: "StoryCraftAnswerTranslations",
                columns: new[] { "StoryCraftAnswerId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCrafts_ClassicAuthorId",
                table: "StoryCrafts",
                column: "ClassicAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCrafts_StoryId",
                table: "StoryCrafts",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTiles_StoryCraftId_TileId",
                table: "StoryCraftTiles",
                columns: new[] { "StoryCraftId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTileTranslations_StoryCraftTileId_LanguageCode",
                table: "StoryCraftTileTranslations",
                columns: new[] { "StoryCraftTileId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTopics_StoryTopicId",
                table: "StoryCraftTopics",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryCraftTranslations_StoryCraftId_LanguageCode",
                table: "StoryCraftTranslations",
                columns: new[] { "StoryCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionAgeGroups_StoryAgeGroupId",
                table: "StoryDefinitionAgeGroups",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitions_ClassicAuthorId",
                table: "StoryDefinitions",
                column: "ClassicAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitions_StoryId",
                table: "StoryDefinitions",
                column: "StoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionTopics_StoryTopicId",
                table: "StoryDefinitionTopics",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryDefinitionTranslations_StoryDefinitionId_LanguageCode",
                table: "StoryDefinitionTranslations",
                columns: new[] { "StoryDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFeedbackPreferences_UserId_StoryId",
                table: "StoryFeedbackPreferences",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFeedbacks_UserId_StoryId",
                table: "StoryFeedbacks",
                columns: new[] { "UserId", "StoryId" },
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
                name: "IX_StoryProgress_TreeConfigurationId",
                table: "StoryProgress",
                column: "TreeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryProgress_UserId_StoryId_TreeConfigurationId",
                table: "StoryProgress",
                columns: new[] { "UserId", "StoryId", "TreeConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublicationAudits_StoryDefinitionId",
                table: "StoryPublicationAudits",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublicationAudits_StoryId",
                table: "StoryPublicationAudits",
                column: "StoryId");

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
                name: "IX_StoryReaders_StoryId",
                table: "StoryReaders",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReaders_UserId_StoryId",
                table: "StoryReaders",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryReviews_StoryId",
                table: "StoryReviews",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryReviews_UserId_StoryId",
                table: "StoryReviews",
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
                name: "IX_StoryTopics_TopicId",
                table: "StoryTopics",
                column: "TopicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryTopicTranslations_StoryTopicId_LanguageCode",
                table: "StoryTopicTranslations",
                columns: new[] { "StoryTopicId", "LanguageCode" },
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
                name: "IX_UserFavoriteStories_StoryDefinitionId",
                table: "UserFavoriteStories",
                column: "StoryDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteStories_UserId_StoryDefinitionId",
                table: "UserFavoriteStories",
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
                name: "StoryAgeGroupTranslations");

            migrationBuilder.DropTable(
                name: "StoryAnswerTokens");

            migrationBuilder.DropTable(
                name: "StoryAnswerTranslations");

            migrationBuilder.DropTable(
                name: "StoryCraftAgeGroups");

            migrationBuilder.DropTable(
                name: "StoryCraftAnswerTokens");

            migrationBuilder.DropTable(
                name: "StoryCraftAnswerTranslations");

            migrationBuilder.DropTable(
                name: "StoryCraftTileTranslations");

            migrationBuilder.DropTable(
                name: "StoryCraftTopics");

            migrationBuilder.DropTable(
                name: "StoryCraftTranslations");

            migrationBuilder.DropTable(
                name: "StoryDefinitionAgeGroups");

            migrationBuilder.DropTable(
                name: "StoryDefinitionTopics");

            migrationBuilder.DropTable(
                name: "StoryDefinitionTranslations");

            migrationBuilder.DropTable(
                name: "StoryFeedbackPreferences");

            migrationBuilder.DropTable(
                name: "StoryFeedbacks");

            migrationBuilder.DropTable(
                name: "StoryHeroUnlocks");

            migrationBuilder.DropTable(
                name: "StoryProgress");

            migrationBuilder.DropTable(
                name: "StoryPublicationAudits");

            migrationBuilder.DropTable(
                name: "StoryPurchases");

            migrationBuilder.DropTable(
                name: "StoryReaders");

            migrationBuilder.DropTable(
                name: "StoryReviews");

            migrationBuilder.DropTable(
                name: "StoryTileTranslations");

            migrationBuilder.DropTable(
                name: "StoryTopicTranslations");

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
                name: "UserFavoriteStories");

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
                name: "StoryCraftAnswers");

            migrationBuilder.DropTable(
                name: "StoryAgeGroups");

            migrationBuilder.DropTable(
                name: "StoryHeroes");

            migrationBuilder.DropTable(
                name: "StoryTopics");

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
                name: "StoryCraftTiles");

            migrationBuilder.DropTable(
                name: "AlchimaliaUsers");

            migrationBuilder.DropTable(
                name: "TreeConfigurations");

            migrationBuilder.DropTable(
                name: "StoryDefinitions");

            migrationBuilder.DropTable(
                name: "StoryCrafts");

            migrationBuilder.DropTable(
                name: "ClassicAuthors");
        }
    }
}
