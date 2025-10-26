using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    public DbSet<StoryMarketplaceInfo> StoryMarketplaceInfos => Set<StoryMarketplaceInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AlchimaliaUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Auth0Id).IsUnique();
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
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

        modelBuilder.Entity<StoryDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => x.StoryId).IsUnique();
            e.HasMany(x => x.Tiles).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
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

        var testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var marianUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        
        modelBuilder.Entity<AlchimaliaUser>().HasData(
            new AlchimaliaUser
            {
                Id = testUserId,
                Auth0Id = "test-user-sub",
                Name = "Test User",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AlchimaliaUser
            {
                Id = marianUserId,
                Auth0Id = "marian-test-sub",
                Name = "Marian",
                Email = "marian@example.com",
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
                UserId = testUserId, 
                Balance = 5, 
                UpdatedAt = DateTime.UtcNow 
            },
            new CreditWallet 
            { 
                UserId = marianUserId, 
                Balance = 5, 
                UpdatedAt = DateTime.UtcNow 
            }
        );



        modelBuilder.Entity<HeroProgress>().HasData(
            new HeroProgress
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserId = testUserId,
                HeroId = "seed",
                HeroType = "HERO_TREE_TRANSFORMATION",
                UnlockedAt = DateTime.UtcNow
            },
            new HeroProgress
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserId = marianUserId,
                HeroId = "seed",
                HeroType = "HERO_TREE_TRANSFORMATION",
                UnlockedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<CreditTransaction>().HasData(
            new CreditTransaction
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                UserId = marianUserId,
                Amount = 15,
                Type = CreditTransactionType.Purchase,
                Reference = "test-purchase-marian",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new CreditTransaction
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                UserId = marianUserId,
                Amount = -5,
                Type = CreditTransactionType.Spend,
                Reference = "test-generation",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        );


        base.OnModelCreating(modelBuilder);
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

        // Story Marketplace Entity Configurations
        modelBuilder.Entity<StoryPurchase>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryMarketplaceInfo>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => x.StoryId).IsUnique();
            e.Property(x => x.Region).HasMaxLength(50);
            e.Property(x => x.AgeRating).HasMaxLength(10);
            e.Property(x => x.Difficulty).HasMaxLength(20);
            e.Property(x => x.Characters).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            ).Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
            e.Property(x => x.Tags).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            ).Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
            e.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
