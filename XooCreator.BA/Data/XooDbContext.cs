using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    public XooDbContext(DbContextOptions<XooDbContext> options) : base(options) { }

    public DbSet<UserAlchimalia> UsersAlchimalia => Set<UserAlchimalia>();
    public DbSet<CreditWallet> CreditWallets => Set<CreditWallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<DiscoveryItem> DiscoveryItems => Set<DiscoveryItem>();
    public DbSet<UserDiscovery> UserDiscoveries => Set<UserDiscovery>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<TreeChoice> TreeChoices => Set<TreeChoice>();
    public DbSet<Creature> Creatures => Set<Creature>();
    public DbSet<Job> Jobs => Set<Job>();

    // Tree of Light data
    public DbSet<TreeProgress> TreeProgress => Set<TreeProgress>();
    public DbSet<StoryProgress> StoryProgress => Set<StoryProgress>();
    // Removed legacy fixed-token table in favor of generic balances
    public DbSet<UserTokenBalance> UserTokenBalances => Set<UserTokenBalance>();
    public DbSet<HeroProgress> HeroProgress => Set<HeroProgress>();
    public DbSet<HeroTreeProgress> HeroTreeProgress => Set<HeroTreeProgress>();
    public DbSet<HeroDefinition> HeroDefinitions => Set<HeroDefinition>();
    
    // Tree of Light Model data
    public DbSet<TreeRegion> TreeRegions => Set<TreeRegion>();
    public DbSet<TreeStoryNode> TreeStoryNodes => Set<TreeStoryNode>();
    public DbSet<TreeUnlockRule> TreeUnlockRules => Set<TreeUnlockRule>();
    public DbSet<TreeConfiguration> TreeConfigurations => Set<TreeConfiguration>();
    
    // Story System data
    public DbSet<StoryDefinition> StoryDefinitions => Set<StoryDefinition>();
    public DbSet<StoryDefinitionTranslation> StoryDefinitionTranslations => Set<StoryDefinitionTranslation>();
    public DbSet<StoryTile> StoryTiles => Set<StoryTile>();
    public DbSet<StoryTileTranslation> StoryTileTranslations => Set<StoryTileTranslation>();
    public DbSet<StoryAnswer> StoryAnswers => Set<StoryAnswer>();
    public DbSet<StoryAnswerTranslation> StoryAnswerTranslations => Set<StoryAnswerTranslation>();
    public DbSet<UserStoryReadProgress> UserStoryReadProgress => Set<UserStoryReadProgress>();
    public DbSet<StoryAnswerToken> StoryAnswerTokens => Set<StoryAnswerToken>();

    // Builder data
    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<BodyPartTranslation> BodyPartTranslations => Set<BodyPartTranslation>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalTranslation> AnimalTranslations => Set<AnimalTranslation>();
    public DbSet<AnimalPartSupport> AnimalPartSupports => Set<AnimalPartSupport>();
    public DbSet<BuilderConfig> BuilderConfigs => Set<BuilderConfig>();
    public DbSet<Region> Regions => Set<Region>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<UserAlchimalia>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Auth0Sub).IsUnique();
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Auth0Sub).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<CreditWallet>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne().HasForeignKey<CreditWallet>(x => x.UserId);
        });

        modelBuilder.Entity<DiscoveryItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.ArmsKey).HasMaxLength(32);
            e.Property(x => x.BodyKey).HasMaxLength(32);
            e.Property(x => x.HeadKey).HasMaxLength(32);
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(x => x.Story).HasMaxLength(10000);
        });

        modelBuilder.Entity<UserDiscovery>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.DiscoveryItemId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.DiscoveryItem).WithMany().HasForeignKey(x => x.DiscoveryItemId);
            e.ToTable("BestiaryDiscovered"); // rename table
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

        // Builder entities
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

        // Seed builder data from JSON files
        SeedBuilderDataFromJson(modelBuilder);

        // Tree of Light configurations
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

        // Legacy UserTokens removed

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
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.Property(x => x.PrerequisitesJson).HasMaxLength(2000);
            e.Property(x => x.RewardsJson).HasMaxLength(2000);
            e.Property(x => x.PositionX).HasColumnType("decimal(10,6)");
            e.Property(x => x.PositionY).HasColumnType("decimal(10,6)");
            e.HasIndex(x => x.Id).IsUnique();
        });

        // Tree of Light Model configurations
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
            // Note: No FK constraints to allow flexible FromId (can be region or story ID)
        });

        modelBuilder.Entity<TreeConfiguration>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
        });

        // Story System configurations
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

        // Seed test users
        var testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var marianUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        
        modelBuilder.Entity<UserAlchimalia>().HasData(
            new UserAlchimalia
            {
                Id = testUserId,
                Auth0Sub = "test-user-sub",
                DisplayName = "Test User",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow
            },
            new UserAlchimalia
            {
                Id = marianUserId,
                Auth0Sub = "marian-test-sub",
                DisplayName = "Marian",
                Email = "marian@example.com",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Config
        modelBuilder.Entity<BuilderConfig>().HasData(new BuilderConfig 
        { 
            Id = 1, 
            BaseUnlockedAnimalIds = "[\"00000000-0000-0000-0000-000000000001\",\"00000000-0000-0000-0000-000000000002\",\"00000000-0000-0000-0000-000000000003\"]", // Bunny, Cat, Giraffe
            BaseUnlockedBodyPartKeys = "[\"head\",\"body\",\"arms\"]" // First 3 body parts
        });

        // Seed test credit wallets and transactions
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
                Balance = 10, 
                UpdatedAt = DateTime.UtcNow 
            }
        );

        // Seed generic balances instead of legacy fixed tokens
        modelBuilder.Entity<UserTokenBalance>().HasData(
            new UserTokenBalance { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), UserId = testUserId, Type = "TreeOfHeroes", Value = "courage", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), UserId = testUserId, Type = "TreeOfHeroes", Value = "curiosity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), UserId = testUserId, Type = "TreeOfHeroes", Value = "thinking", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), UserId = testUserId, Type = "TreeOfHeroes", Value = "creativity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), UserId = testUserId, Type = "TreeOfHeroes", Value = "safety", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), UserId = marianUserId, Type = "TreeOfHeroes", Value = "courage", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), UserId = marianUserId, Type = "TreeOfHeroes", Value = "curiosity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), UserId = marianUserId, Type = "TreeOfHeroes", Value = "thinking", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), UserId = marianUserId, Type = "TreeOfHeroes", Value = "creativity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), UserId = marianUserId, Type = "TreeOfHeroes", Value = "safety", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Note: seed is not unlocked in HeroTreeProgress by default
        // In the old system, seed was only transformed (in HeroProgress), not unlocked in tree progress
        // Base heroes get unlocked when seed is transformed

        // Seed test hero progress (seed transformed by default)
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

        // Seed test credit transactions (simulate purchases)
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

        // Note: Tree of Light Model seeding moved to TreeOfLightService.SeedTreeModel()
        // for better control and avoiding FK constraint issues during migrations

        base.OnModelCreating(modelBuilder);
    }

    private void SeedBuilderDataFromJson(ModelBuilder modelBuilder)
    {
        try
        {
            // Seed RO (default)
            var seedService = new SeedDataService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "ro-ro", "LaboratoryOfImagination"));
            
            // Load data synchronously since we're in OnModelCreating
            var bodyParts = LoadDataSync(() => seedService.LoadBodyPartsAsync());
            var regions = LoadDataSync(() => seedService.LoadRegionsAsync());
            var animals = LoadDataSync(() => seedService.LoadAnimalsAsync());
            var animalPartSupports = LoadDataSync(() => seedService.LoadAnimalPartSupportsAsync());

            // Seed the data (RO base)
            modelBuilder.Entity<BodyPart>().HasData(bodyParts);
            modelBuilder.Entity<Region>().HasData(regions);
            modelBuilder.Entity<Animal>().HasData(animals);
            modelBuilder.Entity<AnimalPartSupport>().HasData(animalPartSupports);

            // Seed EN translations if folder exists (mirror or translated values)
            var enPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "en-us", "LaboratoryOfImagination");
            if (Directory.Exists(enPath))
            {
                var enSeed = new SeedDataService(enPath);
                var bodyPartsEn = LoadDataSync(() => enSeed.LoadBodyPartsAsync());
                var animalsEn = LoadDataSync(() => enSeed.LoadAnimalsAsync());

                // BodyPart translations
                var bodyPartTranslations = bodyPartsEn.Select(bp => new BodyPartTranslation
                {
                    Id = Guid.NewGuid(),
                    BodyPartKey = bp.Key,
                    LanguageCode = "en-us",
                    Name = bp.Name
                }).ToArray();
                modelBuilder.Entity<BodyPartTranslation>().HasData(bodyPartTranslations);

                // Animal translations
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
            // Log the exception or handle it appropriately
            // For now, we'll throw to make the issue visible
            throw new InvalidOperationException("Failed to load seed data from JSON files", ex);
        }
    }

    private static T LoadDataSync<T>(Func<Task<T>> asyncOperation)
    {
        return Task.Run(async () => await asyncOperation()).GetAwaiter().GetResult();
    }
}
