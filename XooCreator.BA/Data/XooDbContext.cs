using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    private readonly string _defaultSchema = "public";

    public XooDbContext(DbContextOptions<XooDbContext> options, IConfiguration configuration) : base(options)
    {
        _defaultSchema = configuration.GetValue<string>("Database:Schema") ?? "public";
    }

    public DbSet<AlchimaliaUser> AlchimaliaUsers => Set<AlchimaliaUser>();
    public DbSet<CreditWallet> CreditWallets => Set<CreditWallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<BestiaryItem> BestiaryItems => Set<BestiaryItem>();
    public DbSet<UserBestiary> UserBestiary => Set<UserBestiary>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<TreeChoice> TreeChoices => Set<TreeChoice>();
    public DbSet<Creature> Creatures => Set<Creature>();
    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<TreeProgress> TreeProgress => Set<TreeProgress>();
    public DbSet<StoryProgress> StoryProgress => Set<StoryProgress>();
    public DbSet<EpicProgress> EpicProgress => Set<EpicProgress>();
    public DbSet<EpicStoryProgress> EpicStoryProgress => Set<EpicStoryProgress>();
    public DbSet<UserTokenBalance> UserTokenBalances => Set<UserTokenBalance>();
    public DbSet<HeroProgress> HeroProgress => Set<HeroProgress>();
    public DbSet<HeroTreeProgress> HeroTreeProgress => Set<HeroTreeProgress>();
    public DbSet<HeroDefinition> HeroDefinitions => Set<HeroDefinition>();
    public DbSet<HeroDefinitionTranslation> HeroDefinitionTranslations => Set<HeroDefinitionTranslation>();
    public DbSet<HeroDefinitionVersion> HeroDefinitionVersions => Set<HeroDefinitionVersion>();
    public DbSet<HeroDefinitionCraft> HeroDefinitionCrafts => Set<HeroDefinitionCraft>();
    public DbSet<HeroDefinitionCraftTranslation> HeroDefinitionCraftTranslations => Set<HeroDefinitionCraftTranslation>();
    public DbSet<HeroDefinitionDefinition> HeroDefinitionDefinitions => Set<HeroDefinitionDefinition>();
    public DbSet<HeroDefinitionDefinitionTranslation> HeroDefinitionDefinitionTranslations => Set<HeroDefinitionDefinitionTranslation>();
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();
    
    public DbSet<TreeRegion> TreeRegions => Set<TreeRegion>();
    public DbSet<TreeStoryNode> TreeStoryNodes => Set<TreeStoryNode>();
    public DbSet<TreeUnlockRule> TreeUnlockRules => Set<TreeUnlockRule>();
    public DbSet<TreeConfiguration> TreeConfigurations => Set<TreeConfiguration>();
    
    // Story Epic - New Architecture (Craft for drafts, Definition for published)
    public DbSet<StoryEpicCraft> StoryEpicCrafts => Set<StoryEpicCraft>();
    public DbSet<StoryEpicCraftTranslation> StoryEpicCraftTranslations => Set<StoryEpicCraftTranslation>();
    public DbSet<StoryEpicCraftRegion> StoryEpicCraftRegions => Set<StoryEpicCraftRegion>();
    public DbSet<StoryEpicCraftStoryNode> StoryEpicCraftStoryNodes => Set<StoryEpicCraftStoryNode>();
    public DbSet<StoryEpicCraftUnlockRule> StoryEpicCraftUnlockRules => Set<StoryEpicCraftUnlockRule>();
    public DbSet<StoryEpicCraftHeroReference> StoryEpicCraftHeroReferences => Set<StoryEpicCraftHeroReference>();
    
    public DbSet<StoryEpicDefinition> StoryEpicDefinitions => Set<StoryEpicDefinition>();
    public DbSet<StoryEpicDefinitionTranslation> StoryEpicDefinitionTranslations => Set<StoryEpicDefinitionTranslation>();
    public DbSet<StoryEpicDefinitionRegion> StoryEpicDefinitionRegions => Set<StoryEpicDefinitionRegion>();
    public DbSet<StoryEpicDefinitionStoryNode> StoryEpicDefinitionStoryNodes => Set<StoryEpicDefinitionStoryNode>();
    public DbSet<StoryEpicDefinitionUnlockRule> StoryEpicDefinitionUnlockRules => Set<StoryEpicDefinitionUnlockRule>();
    
    // Story Epic - Independent Regions and Heroes (Craft/Definition pattern)
    public DbSet<StoryRegionCraft> StoryRegionCrafts => Set<StoryRegionCraft>();
    public DbSet<StoryRegionCraftTranslation> StoryRegionCraftTranslations => Set<StoryRegionCraftTranslation>();
    public DbSet<StoryRegionDefinition> StoryRegionDefinitions => Set<StoryRegionDefinition>();
    public DbSet<StoryRegionDefinitionTranslation> StoryRegionDefinitionTranslations => Set<StoryRegionDefinitionTranslation>();
    public DbSet<EpicHeroCraft> EpicHeroCrafts => Set<EpicHeroCraft>();
    public DbSet<EpicHeroCraftTranslation> EpicHeroCraftTranslations => Set<EpicHeroCraftTranslation>();
    public DbSet<EpicHeroDefinition> EpicHeroDefinitions => Set<EpicHeroDefinition>();
    public DbSet<EpicHeroDefinitionTranslation> EpicHeroDefinitionTranslations => Set<EpicHeroDefinitionTranslation>();
    public DbSet<StoryEpicRegionReference> StoryEpicRegionReferences => Set<StoryEpicRegionReference>();
    public DbSet<StoryEpicHeroReference> StoryEpicHeroReferences => Set<StoryEpicHeroReference>();
    
    // Version Jobs
    public DbSet<HeroVersionJob> HeroVersionJobs => Set<HeroVersionJob>();
    public DbSet<RegionVersionJob> RegionVersionJobs => Set<RegionVersionJob>();
    
    public DbSet<StoryDefinition> StoryDefinitions => Set<StoryDefinition>();
    public DbSet<StoryDefinitionTranslation> StoryDefinitionTranslations => Set<StoryDefinitionTranslation>();
    public DbSet<StoryTile> StoryTiles => Set<StoryTile>();
    public DbSet<StoryTileTranslation> StoryTileTranslations => Set<StoryTileTranslation>();
    public DbSet<StoryAnswer> StoryAnswers => Set<StoryAnswer>();
    public DbSet<StoryAnswerTranslation> StoryAnswerTranslations => Set<StoryAnswerTranslation>();
    public DbSet<UserStoryReadProgress> UserStoryReadProgress => Set<UserStoryReadProgress>();
    public DbSet<UserStoryReadHistory> UserStoryReadHistory => Set<UserStoryReadHistory>();
    public DbSet<StoryAnswerToken> StoryAnswerTokens => Set<StoryAnswerToken>();
    public DbSet<StoryQuizAnswer> StoryQuizAnswers => Set<StoryQuizAnswer>();
    public DbSet<StoryEvaluationResult> StoryEvaluationResults => Set<StoryEvaluationResult>();
    public DbSet<StoryHero> StoryHeroes => Set<StoryHero>();
    public DbSet<StoryHeroTranslation> StoryHeroTranslations => Set<StoryHeroTranslation>();
    public DbSet<StoryHeroVersion> StoryHeroVersions => Set<StoryHeroVersion>();
    public DbSet<StoryHeroUnlock> StoryHeroUnlocks => Set<StoryHeroUnlock>();
    public DbSet<HeroMessage> HeroMessages => Set<HeroMessage>();
    public DbSet<HeroClickMessage> HeroClickMessages => Set<HeroClickMessage>();

    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<BodyPartTranslation> BodyPartTranslations => Set<BodyPartTranslation>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalTranslation> AnimalTranslations => Set<AnimalTranslation>();
    public DbSet<AnimalPartSupport> AnimalPartSupports => Set<AnimalPartSupport>();
    public DbSet<AnimalVersion> AnimalVersions => Set<AnimalVersion>();
    public DbSet<AnimalCraft> AnimalCrafts => Set<AnimalCraft>();
    public DbSet<AnimalCraftTranslation> AnimalCraftTranslations => Set<AnimalCraftTranslation>();
    public DbSet<AnimalCraftPartSupport> AnimalCraftPartSupports => Set<AnimalCraftPartSupport>();
    public DbSet<AnimalHybridCraftPart> AnimalHybridCraftParts => Set<AnimalHybridCraftPart>();
    public DbSet<AnimalDefinition> AnimalDefinitions => Set<AnimalDefinition>();
    public DbSet<AnimalDefinitionTranslation> AnimalDefinitionTranslations => Set<AnimalDefinitionTranslation>();
    public DbSet<AnimalDefinitionPartSupport> AnimalDefinitionPartSupports => Set<AnimalDefinitionPartSupport>();
    public DbSet<AnimalHybridDefinitionPart> AnimalHybridDefinitionParts => Set<AnimalHybridDefinitionPart>();
    public DbSet<BuilderConfig> BuilderConfigs => Set<BuilderConfig>();
    public DbSet<Region> Regions => Set<Region>();

    // Tree of Heroes configs (Craft/Definition)
    public DbSet<TreeOfHeroesConfigCraft> TreeOfHeroesConfigCrafts => Set<TreeOfHeroesConfigCraft>();
    public DbSet<TreeOfHeroesConfigCraftNode> TreeOfHeroesConfigCraftNodes => Set<TreeOfHeroesConfigCraftNode>();
    public DbSet<TreeOfHeroesConfigCraftEdge> TreeOfHeroesConfigCraftEdges => Set<TreeOfHeroesConfigCraftEdge>();
    public DbSet<TreeOfHeroesConfigDefinition> TreeOfHeroesConfigDefinitions => Set<TreeOfHeroesConfigDefinition>();
    public DbSet<TreeOfHeroesConfigDefinitionNode> TreeOfHeroesConfigDefinitionNodes => Set<TreeOfHeroesConfigDefinitionNode>();
    public DbSet<TreeOfHeroesConfigDefinitionEdge> TreeOfHeroesConfigDefinitionEdges => Set<TreeOfHeroesConfigDefinitionEdge>();
    
    // Story Marketplace
    public DbSet<StoryPurchase> StoryPurchases => Set<StoryPurchase>();
    public DbSet<StoryReview> StoryReviews => Set<StoryReview>();
    public DbSet<EpicReview> EpicReviews => Set<EpicReview>();
    public DbSet<UserFavoriteStories> UserFavoriteStories => Set<UserFavoriteStories>();
    public DbSet<UserFavoriteEpics> UserFavoriteEpics => Set<UserFavoriteEpics>();
    public DbSet<StoryReader> StoryReaders => Set<StoryReader>();
    public DbSet<EpicReader> EpicReaders => Set<EpicReader>();
    public DbSet<StoryLike> StoryLikes => Set<StoryLike>();
    
    // User Story Relations
    public DbSet<UserOwnedStories> UserOwnedStories => Set<UserOwnedStories>();
    public DbSet<UserCreatedStories> UserCreatedStories => Set<UserCreatedStories>();
    public DbSet<StoryPublicationAudit> StoryPublicationAudits => Set<StoryPublicationAudit>();
    public DbSet<StoryPublishChangeLog> StoryPublishChangeLogs => Set<StoryPublishChangeLog>();
    public DbSet<StoryAssetLink> StoryAssetLinks => Set<StoryAssetLink>();
    
    // Region and Hero Delta Publish
    public DbSet<RegionPublishChangeLog> RegionPublishChangeLogs => Set<RegionPublishChangeLog>();
    public DbSet<HeroPublishChangeLog> HeroPublishChangeLogs => Set<HeroPublishChangeLog>();
    public DbSet<RegionAssetLink> RegionAssetLinks => Set<RegionAssetLink>();
    public DbSet<HeroAssetLink> HeroAssetLinks => Set<HeroAssetLink>();
    
    // Epic Delta Publish
    public DbSet<EpicPublishChangeLog> EpicPublishChangeLogs => Set<EpicPublishChangeLog>();
    public DbSet<EpicAssetLink> EpicAssetLinks => Set<EpicAssetLink>();

    // Hero Definition Delta Publish
    public DbSet<HeroDefinitionPublishChangeLog> HeroDefinitionPublishChangeLogs => Set<HeroDefinitionPublishChangeLog>();

    // Animal Delta Publish
    public DbSet<AnimalPublishChangeLog> AnimalPublishChangeLogs => Set<AnimalPublishChangeLog>();
    
    // Background Jobs
    public DbSet<HeroDefinitionVersionJob> HeroDefinitionVersionJobs => Set<HeroDefinitionVersionJob>();
    public DbSet<AnimalVersionJob> AnimalVersionJobs => Set<AnimalVersionJob>();

    public DbSet<StoryPublishJob> StoryPublishJobs => Set<StoryPublishJob>();
    public DbSet<StoryVersionJob> StoryVersionJobs => Set<StoryVersionJob>();
    public DbSet<EpicVersionJob> EpicVersionJobs => Set<EpicVersionJob>();
    public DbSet<EpicPublishJob> EpicPublishJobs => Set<EpicPublishJob>();
    public DbSet<StoryImportJob> StoryImportJobs => Set<StoryImportJob>();
    public DbSet<StoryForkJob> StoryForkJobs => Set<StoryForkJob>();
    public DbSet<StoryForkAssetJob> StoryForkAssetJobs => Set<StoryForkAssetJob>();
    public DbSet<StoryExportJob> StoryExportJobs => Set<StoryExportJob>();
    public DbSet<StoryDocumentExportJob> StoryDocumentExportJobs => Set<StoryDocumentExportJob>();
    public DbSet<StoryCraft> StoryCrafts => Set<StoryCraft>();
    public DbSet<StoryCraftTranslation> StoryCraftTranslations => Set<StoryCraftTranslation>();
    public DbSet<StoryCraftTile> StoryCraftTiles => Set<StoryCraftTile>();
    public DbSet<StoryCraftTileTranslation> StoryCraftTileTranslations => Set<StoryCraftTileTranslation>();
    public DbSet<StoryCraftAnswer> StoryCraftAnswers => Set<StoryCraftAnswer>();
    public DbSet<StoryCraftAnswerTranslation> StoryCraftAnswerTranslations => Set<StoryCraftAnswerTranslation>();
    public DbSet<StoryCraftAnswerToken> StoryCraftAnswerTokens => Set<StoryCraftAnswerToken>();
    
    // Story Topics and Age Groups
    public DbSet<StoryTopic> StoryTopics => Set<StoryTopic>();
    public DbSet<StoryTopicTranslation> StoryTopicTranslations => Set<StoryTopicTranslation>();
    public DbSet<StoryCraftTopic> StoryCraftTopics => Set<StoryCraftTopic>();
    public DbSet<StoryDefinitionTopic> StoryDefinitionTopics => Set<StoryDefinitionTopic>();
    public DbSet<StoryCraftUnlockedHero> StoryCraftUnlockedHeroes => Set<StoryCraftUnlockedHero>();
    public DbSet<StoryDefinitionUnlockedHero> StoryDefinitionUnlockedHeroes => Set<StoryDefinitionUnlockedHero>();
    public DbSet<StoryAgeGroup> StoryAgeGroups => Set<StoryAgeGroup>();
    public DbSet<StoryAgeGroupTranslation> StoryAgeGroupTranslations => Set<StoryAgeGroupTranslation>();
    public DbSet<StoryCraftAgeGroup> StoryCraftAgeGroups => Set<StoryCraftAgeGroup>();
    public DbSet<StoryDefinitionAgeGroup> StoryDefinitionAgeGroups => Set<StoryDefinitionAgeGroup>();
    
    // Classic Authors
    public DbSet<ClassicAuthor> ClassicAuthors => Set<ClassicAuthor>();
    
    // Story Feedback
    public DbSet<StoryFeedback> StoryFeedbacks => Set<StoryFeedback>();
    public DbSet<StoryFeedbackPreference> StoryFeedbackPreferences => Set<StoryFeedbackPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_defaultSchema);
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // Apply all entity configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Explicitly set schema for PlatformSettings table
        modelBuilder.Entity<PlatformSetting>().ToTable("PlatformSettings", _defaultSchema);

        // Seed data methods removed (JSON is no longer a runtime source)

        // Seed data for AlchimaliaUser and related entities
        var systemAdminUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        modelBuilder.Entity<AlchimaliaUser>().HasData(
            new AlchimaliaUser
            {
                Id = systemAdminUserId,
                Auth0Id = "alchimalia-admin-sub",
                Name = "Marian Teacher",
                FirstName = "Marian",
                LastName = "Teacher",
                Email = "alchimalia@admin.com",
                Role = Enums.UserRole.Admin,
                Roles = new List<UserRole> { Enums.UserRole.Admin },
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<BuilderConfig>().HasData(new BuilderConfig 
        { 
            Id = 1, 
            BaseUnlockedAnimalIds = "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]", // Bunny, Cat, Giraffe
            BaseUnlockedBodyPartKeys = "[\"head\",\"body\",\"arms\"]" // First 3 body parts
        });

        modelBuilder.Entity<CreditWallet>().HasData(
            new CreditWallet 
            { 
                UserId = systemAdminUserId, 
                Balance = 1000, 
                UpdatedAt = DateTime.UtcNow 
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    // Removed all individual entity configurations - they are now in separate configuration files
    // in the Configurations folder and are applied via ApplyConfigurationsFromAssembly

    // JSON-based seeding removed; DB is the single source of truth.

    // REMOVED: SeedIndependentStoriesDataFromJson method
    // Independent stories are now seeded via StoriesRepository.SeedIndependentStoriesAsync()
    // which uses SaveChanges() instead of HasData() to allow navigation properties.
    // This method was removed because HasData() cannot handle entities with navigation properties.

}
