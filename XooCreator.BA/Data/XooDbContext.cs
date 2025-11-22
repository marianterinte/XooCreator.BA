using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    public XooDbContext(DbContextOptions<XooDbContext> options) : base(options) { }

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
    public DbSet<UserTokenBalance> UserTokenBalances => Set<UserTokenBalance>();
    public DbSet<HeroProgress> HeroProgress => Set<HeroProgress>();
    public DbSet<HeroTreeProgress> HeroTreeProgress => Set<HeroTreeProgress>();
    public DbSet<HeroDefinition> HeroDefinitions => Set<HeroDefinition>();
    public DbSet<HeroDefinitionTranslation> HeroDefinitionTranslations => Set<HeroDefinitionTranslation>();
    
    public DbSet<TreeRegion> TreeRegions => Set<TreeRegion>();
    public DbSet<TreeStoryNode> TreeStoryNodes => Set<TreeStoryNode>();
    public DbSet<TreeUnlockRule> TreeUnlockRules => Set<TreeUnlockRule>();
    public DbSet<TreeConfiguration> TreeConfigurations => Set<TreeConfiguration>();
    
    public DbSet<StoryDefinition> StoryDefinitions => Set<StoryDefinition>();
    public DbSet<StoryDefinitionTranslation> StoryDefinitionTranslations => Set<StoryDefinitionTranslation>();
    public DbSet<StoryTile> StoryTiles => Set<StoryTile>();
    public DbSet<StoryTileTranslation> StoryTileTranslations => Set<StoryTileTranslation>();
    public DbSet<StoryAnswer> StoryAnswers => Set<StoryAnswer>();
    public DbSet<StoryAnswerTranslation> StoryAnswerTranslations => Set<StoryAnswerTranslation>();
    public DbSet<UserStoryReadProgress> UserStoryReadProgress => Set<UserStoryReadProgress>();
    public DbSet<StoryAnswerToken> StoryAnswerTokens => Set<StoryAnswerToken>();
    public DbSet<StoryHero> StoryHeroes => Set<StoryHero>();
    public DbSet<StoryHeroUnlock> StoryHeroUnlocks => Set<StoryHeroUnlock>();
    public DbSet<HeroMessage> HeroMessages => Set<HeroMessage>();
    public DbSet<HeroClickMessage> HeroClickMessages => Set<HeroClickMessage>();

    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<BodyPartTranslation> BodyPartTranslations => Set<BodyPartTranslation>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalTranslation> AnimalTranslations => Set<AnimalTranslation>();
    public DbSet<AnimalPartSupport> AnimalPartSupports => Set<AnimalPartSupport>();
    public DbSet<BuilderConfig> BuilderConfigs => Set<BuilderConfig>();
    public DbSet<Region> Regions => Set<Region>();
    
    // Story Marketplace
    public DbSet<StoryPurchase> StoryPurchases => Set<StoryPurchase>();
    public DbSet<StoryReview> StoryReviews => Set<StoryReview>();
    public DbSet<UserFavoriteStories> UserFavoriteStories => Set<UserFavoriteStories>();
    public DbSet<StoryReader> StoryReaders => Set<StoryReader>();
    
    // User Story Relations
    public DbSet<UserOwnedStories> UserOwnedStories => Set<UserOwnedStories>();
    public DbSet<UserCreatedStories> UserCreatedStories => Set<UserCreatedStories>();
    public DbSet<StoryPublicationAudit> StoryPublicationAudits => Set<StoryPublicationAudit>();
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
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AlchimaliaUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Auth0Id).IsUnique();
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Role).IsRequired();
            // Configure Roles as PostgreSQL array
            e.Property(x => x.Roles)
                .HasConversion(
                    v => v.Select(r => (int)r).ToArray(),
                    v => v.Select(r => (UserRole)r).ToList())
                .HasColumnType("integer[]");
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Auth0Id).HasMaxLength(256).IsRequired();
            e.Property(x => x.Picture).HasMaxLength(512);
        });

        modelBuilder.Entity<CreditWallet>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne().HasForeignKey<CreditWallet>(x => x.UserId);
        });

        modelBuilder.Entity<BestiaryItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.ArmsKey).HasMaxLength(32);
            e.Property(x => x.BodyKey).HasMaxLength(32);
            e.Property(x => x.HeadKey).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(x => x.Story).HasMaxLength(10000);
        });

        modelBuilder.Entity<UserBestiary>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.BestiaryType).HasMaxLength(32).IsRequired();
            e.HasIndex(x => new { x.UserId, x.BestiaryItemId, x.BestiaryType }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.BestiaryItem).WithMany().HasForeignKey(x => x.BestiaryItemId);
            e.ToTable("UserBestiary");
        });

        modelBuilder.Entity<CreditTransaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).IsRequired();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Tree>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<TreeChoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tree).WithMany(x => x.Choices).HasForeignKey(x => x.TreeId);
            e.HasIndex(x => new { x.TreeId, x.Tier }).IsUnique();
        });

        modelBuilder.Entity<Creature>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Tree).WithMany().HasForeignKey(x => x.TreeId);
        });

        modelBuilder.Entity<Job>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(24);
            e.Property(x => x.Status).HasMaxLength(24);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<BodyPart>(e =>
        {
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.Property(x => x.Image).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<BodyPartTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.BodyPartKey, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.BodyPart)
                .WithMany(b => b.Translations)
                .HasForeignKey(x => x.BodyPartKey)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Region>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Animal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(64).IsRequired();
            e.Property(x => x.Src).HasMaxLength(256).IsRequired();
            e.Property(x => x.IsHybrid).HasDefaultValue(false);
            e.HasOne(x => x.Region).WithMany(x => x.Animals).HasForeignKey(x => x.RegionId);
        });

        modelBuilder.Entity<AnimalTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.AnimalId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.Animal)
                .WithMany(a => a.Translations)
                .HasForeignKey(x => x.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnimalPartSupport>(e =>
        {
            e.HasKey(x => new { x.AnimalId, x.PartKey });
            e.HasOne(x => x.Animal).WithMany(x => x.SupportedParts).HasForeignKey(x => x.AnimalId);
            e.HasOne(x => x.Part).WithMany().HasForeignKey(x => x.PartKey);
        });

        modelBuilder.Entity<BuilderConfig>(e =>
        {
            e.HasKey(x => x.Id);
        });

        SeedBuilderDataFromJson(modelBuilder);

        SeedStoryHeroesDataFromJson(modelBuilder);
        
        SeedHeroMessagesDataFromJson(modelBuilder);
        
        SeedStoryHeroDefinitions(modelBuilder);
        
        // Independent stories are seeded via StoriesRepository.SeedIndependentStoriesAsync()
        // which uses SaveChanges() instead of HasData() to allow navigation properties
        // SeedIndependentStoriesDataFromJson(modelBuilder);

        modelBuilder.Entity<TreeProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.RegionId, x.TreeConfigurationId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
        });

        modelBuilder.Entity<StoryProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId, x.TreeConfigurationId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
        });


        modelBuilder.Entity<UserTokenBalance>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Type).HasMaxLength(64).IsRequired();
            e.Property(x => x.Value).HasMaxLength(128).IsRequired();
            e.HasIndex(x => new { x.UserId, x.Type, x.Value }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<HeroProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.HeroId, x.HeroType }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<HeroTreeProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.NodeId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<HeroDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.Property(x => x.PrerequisitesJson).HasMaxLength(2000);
            e.Property(x => x.RewardsJson).HasMaxLength(2000);
            e.Property(x => x.PositionX).HasColumnType("decimal(10,6)");
            e.Property(x => x.PositionY).HasColumnType("decimal(10,6)");
            e.HasIndex(x => x.Id).IsUnique();
        });

        modelBuilder.Entity<HeroDefinitionTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.HeroDefinitionId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.HeroDefinition)
                .WithMany(s => s.Translations)
                .HasForeignKey(x => x.HeroDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TreeRegion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(50);
            e.Property(x => x.Label).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.Id, x.TreeConfigurationId }).IsUnique();
            e.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
        });

        modelBuilder.Entity<TreeStoryNode>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
            e.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.StoryId, x.RegionId, x.TreeConfigurationId }).IsUnique();
            e.HasOne(x => x.Region).WithMany(x => x.Stories).HasForeignKey(x => x.RegionId);
            e.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId);
            e.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
        });

        modelBuilder.Entity<TreeUnlockRule>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Type).HasMaxLength(20).IsRequired();
            e.Property(x => x.FromId).HasMaxLength(100).IsRequired();
            e.Property(x => x.ToRegionId).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
        });

        modelBuilder.Entity<TreeConfiguration>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<StoryReader>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
            e.Property(x => x.AcquiredAt).IsRequired();
            e.Property(x => x.AcquisitionSource).HasConversion<int>();
            e.HasIndex(x => x.StoryId);
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => x.StoryId).IsUnique();
            e.HasMany(x => x.Tiles).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Topics).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.AgeGroups).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ClassicAuthor)
                .WithMany(x => x.StoryDefinitions)
                .HasForeignKey(x => x.ClassicAuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StoryDefinitionTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryDefinitionId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryDefinition)
                .WithMany(s => s.Translations)
                .HasForeignKey(x => x.StoryDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryTile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryDefinitionId, x.TileId }).IsUnique();
            e.HasOne(x => x.StoryDefinition).WithMany(x => x.Tiles).HasForeignKey(x => x.StoryDefinitionId);
            e.HasMany(x => x.Answers).WithOne(x => x.StoryTile).HasForeignKey(x => x.StoryTileId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryTileTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryTileId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryTile)
                .WithMany(t => t.Translations)
                .HasForeignKey(x => x.StoryTileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryAnswer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryTileId, x.AnswerId }).IsUnique();
            e.HasOne(x => x.StoryTile).WithMany(x => x.Answers).HasForeignKey(x => x.StoryTileId);
            e.HasMany(x => x.Tokens).WithOne(x => x.StoryAnswer).HasForeignKey(x => x.StoryAnswerId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryAnswerTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryAnswerId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryAnswer)
                .WithMany(a => a.Translations)
                .HasForeignKey(x => x.StoryAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryAnswerToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Type).HasMaxLength(64).IsRequired();
            e.Property(x => x.Value).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<UserStoryReadProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId, x.TileId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<StoryHero>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
            e.Property(x => x.ImageUrl).HasMaxLength(500);
            e.Property(x => x.UnlockConditionJson).HasMaxLength(2000);
            e.HasIndex(x => x.HeroId).IsUnique();
            e.HasMany(x => x.StoryUnlocks).WithOne(x => x.StoryHero).HasForeignKey(x => x.StoryHeroId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryHeroUnlock>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.StoryHeroId, x.StoryId }).IsUnique();
            e.HasOne(x => x.StoryHero).WithMany(x => x.StoryUnlocks).HasForeignKey(x => x.StoryHeroId);
        });

        modelBuilder.Entity<HeroMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
            e.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
            e.Property(x => x.MessageKey).HasMaxLength(200).IsRequired();
            e.HasIndex(x => new { x.HeroId, x.RegionId }).IsUnique();
        });

        modelBuilder.Entity<HeroClickMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
            e.Property(x => x.MessageKey).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.HeroId).IsUnique();
        });

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

        // Story Marketplace Entity Configurations
        modelBuilder.Entity<StoryPurchase>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
        });

        // Story Reviews Entity Configuration
        modelBuilder.Entity<StoryReview>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
            e.Property(x => x.Rating).IsRequired();
            e.Property(x => x.Comment).HasMaxLength(2000);
            e.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserFavoriteStories>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.AddedAt).IsRequired();
            e.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.StoryDefinition)
                .WithMany()
                .HasForeignKey(x => x.StoryDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Removed StoryMarketplaceInfo entity mapping

        // User Owned Stories Configuration
        modelBuilder.Entity<UserOwnedStories>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
            e.Property(x => x.PurchasePrice).HasColumnType("decimal(10,2)");
            e.Property(x => x.PurchaseReference).HasMaxLength(100);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        // User Created Stories Configuration
        modelBuilder.Entity<UserCreatedStories>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
            e.Property(x => x.CreationNotes).HasMaxLength(1000);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        // Story Feedback Configuration
        modelBuilder.Entity<StoryFeedback>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.FeedbackText).HasMaxLength(5000).IsRequired();
            // Configure lists as PostgreSQL JSON arrays
            e.Property(x => x.WhatLiked)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("jsonb");
            e.Property(x => x.WhatDisliked)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("jsonb");
            e.Property(x => x.WhatCouldBeBetter)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("jsonb");
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique(); // One feedback per user per story
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Story Feedback Preference Configuration
        modelBuilder.Entity<StoryFeedbackPreference>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique(); // One preference per user per story
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoryCraft>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
            e.HasIndex(x => x.StoryId).IsUnique();
            e.HasMany(x => x.Translations).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Tiles).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Topics).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.AgeGroups).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ClassicAuthor)
                .WithMany()
                .HasForeignKey(x => x.ClassicAuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StoryCraftTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryCraftId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryCraft)
                .WithMany(s => s.Translations)
                .HasForeignKey(x => x.StoryCraftId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftTile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryCraftId, x.TileId }).IsUnique();
            e.HasOne(x => x.StoryCraft).WithMany(x => x.Tiles).HasForeignKey(x => x.StoryCraftId);
            e.HasMany(x => x.Answers).WithOne(x => x.StoryCraftTile).HasForeignKey(x => x.StoryCraftTileId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Translations).WithOne(x => x.StoryCraftTile).HasForeignKey(x => x.StoryCraftTileId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftTileTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryCraftTileId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryCraftTile)
                .WithMany(t => t.Translations)
                .HasForeignKey(x => x.StoryCraftTileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftAnswer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryCraftTileId, x.AnswerId }).IsUnique();
            e.HasOne(x => x.StoryCraftTile).WithMany(x => x.Answers).HasForeignKey(x => x.StoryCraftTileId);
            e.HasMany(x => x.Tokens).WithOne(x => x.StoryCraftAnswer).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Translations).WithOne(x => x.StoryCraftAnswer).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftAnswerTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryCraftAnswerId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryCraftAnswer)
                .WithMany(a => a.Translations)
                .HasForeignKey(x => x.StoryCraftAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftAnswerToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Type).HasMaxLength(64).IsRequired();
            e.Property(x => x.Value).HasMaxLength(128).IsRequired();
            e.HasOne(x => x.StoryCraftAnswer).WithMany(x => x.Tokens).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
        });

        // Story Topics Configuration
        modelBuilder.Entity<StoryTopic>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.TopicId).HasMaxLength(100).IsRequired();
            e.Property(x => x.DimensionId).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.TopicId).IsUnique();
            e.HasMany(x => x.Translations).WithOne(x => x.StoryTopic).HasForeignKey(x => x.StoryTopicId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryTopicTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryTopicId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryTopic)
                .WithMany(t => t.Translations)
                .HasForeignKey(x => x.StoryTopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryCraftTopic>(e =>
        {
            e.HasKey(x => new { x.StoryCraftId, x.StoryTopicId });
            e.HasOne(x => x.StoryCraft).WithMany(x => x.Topics).HasForeignKey(x => x.StoryCraftId);
            e.HasOne(x => x.StoryTopic).WithMany(x => x.StoryCrafts).HasForeignKey(x => x.StoryTopicId);
        });

        modelBuilder.Entity<StoryDefinitionTopic>(e =>
        {
            e.HasKey(x => new { x.StoryDefinitionId, x.StoryTopicId });
            e.HasOne(x => x.StoryDefinition).WithMany(x => x.Topics).HasForeignKey(x => x.StoryDefinitionId);
            e.HasOne(x => x.StoryTopic).WithMany(x => x.StoryDefinitions).HasForeignKey(x => x.StoryTopicId);
        });

        // Story Age Groups Configuration
        modelBuilder.Entity<StoryAgeGroup>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.AgeGroupId).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.AgeGroupId).IsUnique();
            e.HasMany(x => x.Translations).WithOne(x => x.StoryAgeGroup).HasForeignKey(x => x.StoryAgeGroupId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryAgeGroupTranslation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.LanguageCode).HasMaxLength(10);
            e.HasIndex(x => new { x.StoryAgeGroupId, x.LanguageCode }).IsUnique();
            e.HasOne(x => x.StoryAgeGroup)
                .WithMany(t => t.Translations)
                .HasForeignKey(x => x.StoryAgeGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassicAuthor>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.AuthorId).HasMaxLength(100).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
            e.HasIndex(x => x.AuthorId).IsUnique();
            e.HasIndex(x => new { x.LanguageCode, x.SortOrder });
        });

        modelBuilder.Entity<StoryCraftAgeGroup>(e =>
        {
            e.HasKey(x => new { x.StoryCraftId, x.StoryAgeGroupId });
            e.HasOne(x => x.StoryCraft).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryCraftId);
            e.HasOne(x => x.StoryAgeGroup).WithMany(x => x.StoryCrafts).HasForeignKey(x => x.StoryAgeGroupId);
        });

        modelBuilder.Entity<StoryDefinitionAgeGroup>(e =>
        {
            e.HasKey(x => new { x.StoryDefinitionId, x.StoryAgeGroupId });
            e.HasOne(x => x.StoryDefinition).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryDefinitionId);
            e.HasOne(x => x.StoryAgeGroup).WithMany(x => x.StoryDefinitions).HasForeignKey(x => x.StoryAgeGroupId);
        });

        modelBuilder.Entity<StoryPublicationAudit>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
            e.Property(x => x.PerformedByEmail).HasMaxLength(256).IsRequired();
            e.Property(x => x.Action).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => x.StoryId);
            e.HasIndex(x => x.StoryDefinitionId);
            e.HasOne(x => x.StoryDefinition)
                .WithMany()
                .HasForeignKey(x => x.StoryDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void SeedBuilderDataFromJson(ModelBuilder modelBuilder)
    {
        try
        {
            var seedService = new SeedDataService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "LaboratoryOfImagination", "i18n", "ro-ro"));
            
            var bodyParts = LoadDataSync(() => seedService.LoadBodyPartsAsync());
            var regions = LoadDataSync(() => seedService.LoadRegionsAsync());
            var animals = LoadDataSync(() => seedService.LoadAnimalsAsync());
            var animalPartSupports = LoadDataSync(() => seedService.LoadAnimalPartSupportsAsync());

            modelBuilder.Entity<BodyPart>().HasData(bodyParts);
            modelBuilder.Entity<Region>().HasData(regions);
            modelBuilder.Entity<Animal>().HasData(animals);
            modelBuilder.Entity<AnimalPartSupport>().HasData(animalPartSupports);

            var enPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "LaboratoryOfImagination", "i18n", "en-us");
            if (Directory.Exists(enPath))
            {
                var enSeed = new SeedDataService(enPath);
                var bodyPartsEn = LoadDataSync(() => enSeed.LoadBodyPartsAsync());
                var animalsEn = LoadDataSync(() => enSeed.LoadAnimalsAsync());

                var bodyPartTranslations = bodyPartsEn.Select(bp => new BodyPartTranslation
                {
                    Id = Guid.NewGuid(),
                    BodyPartKey = bp.Key,
                    LanguageCode = "en-us",
                    Name = bp.Name
                }).ToArray();
                modelBuilder.Entity<BodyPartTranslation>().HasData(bodyPartTranslations);

                var animalTranslations = animalsEn.Select(a => new AnimalTranslation
                {
                    Id = Guid.NewGuid(),
                    AnimalId = a.Id,
                    LanguageCode = "en-us",
                    Label = a.Label
                }).ToArray();
                modelBuilder.Entity<AnimalTranslation>().HasData(animalTranslations);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load seed data from JSON files", ex);
        }
    }

    private static T LoadDataSync<T>(Func<Task<T>> asyncOperation)
    {
        return Task.Run(async () => await asyncOperation()).GetAwaiter().GetResult();
    }

    private void SeedStoryHeroesDataFromJson(ModelBuilder modelBuilder)
    {
        try
        {
            var seedService = new SeedDataService();
            
            var storyHeroes = LoadDataSync(() => seedService.LoadStoryHeroesAsync());
            
            modelBuilder.Entity<StoryHero>().HasData(storyHeroes);
            
            var storyHeroUnlocks = LoadDataSync(() => seedService.LoadStoryHeroUnlocksAsync());
            modelBuilder.Entity<StoryHeroUnlock>().HasData(storyHeroUnlocks);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load story heroes seed data from JSON files", ex);
        }
    }

    private void SeedHeroMessagesDataFromJson(ModelBuilder modelBuilder)
    {
        try
        {
            var seedService = new SeedDataService();
            
            var heroMessages = LoadDataSync(() => seedService.LoadHeroMessagesAsync());
            modelBuilder.Entity<HeroMessage>().HasData(heroMessages);
            
            var heroClickMessages = LoadDataSync(() => seedService.LoadHeroClickMessagesAsync());
            modelBuilder.Entity<HeroClickMessage>().HasData(heroClickMessages);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load hero messages seed data from JSON files", ex);
        }
    }

    private void SeedStoryHeroDefinitions(ModelBuilder modelBuilder)
    {
        var heroDefinitions = new List<HeroDefinition>
        {
            new HeroDefinition
            {
                Id = "puf-puf",
                Type = "STORY_HERO",
                CourageCost = 0,
                CuriosityCost = 0,
                ThinkingCost = 0,
                CreativityCost = 0,
                SafetyCost = 0,
                PrerequisitesJson = "[]",
                RewardsJson = "[]",
                IsUnlocked = false,
                PositionX = 0,
                PositionY = 0,
                Image = "images/heroes/pufpufblink.gif",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HeroDefinition
            {
                Id = "linkaro",
                Type = "STORY_HERO",
                CourageCost = 0,
                CuriosityCost = 0,
                ThinkingCost = 0,
                CreativityCost = 0,
                SafetyCost = 0,
                PrerequisitesJson = "[]",
                RewardsJson = "[]",
                IsUnlocked = false,
                PositionX = 0,
                PositionY = 0,
                Image = "images/heroes/linkaro.png",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new HeroDefinition
            {
                Id = "grubot",
                Type = "STORY_HERO",
                CourageCost = 0,
                CuriosityCost = 0,
                ThinkingCost = 0,
                CreativityCost = 0,
                SafetyCost = 0,
                PrerequisitesJson = "[]",
                RewardsJson = "[]",
                IsUnlocked = false,
                PositionX = 0,
                PositionY = 0,
                Image = "images/heroes/grubot.png",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<HeroDefinition>().HasData(heroDefinitions);
    }

    // REMOVED: SeedIndependentStoriesDataFromJson method
    // Independent stories are now seeded via StoriesRepository.SeedIndependentStoriesAsync()
    // which uses SaveChanges() instead of HasData() to allow navigation properties.
    // This method was removed because HasData() cannot handle entities with navigation properties.

}
