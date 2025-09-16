using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    public XooDbContext(DbContextOptions<XooDbContext> options) : base(options) { }

    public DbSet<UserAlchimalia> UsersAlchimalia => Set<UserAlchimalia>();
    public DbSet<CreditWallet> CreditWallets => Set<CreditWallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<TreeChoice> TreeChoices => Set<TreeChoice>();
    public DbSet<Creature> Creatures => Set<Creature>();
    public DbSet<Job> Jobs => Set<Job>();

    // Tree of Light data
    public DbSet<TreeProgress> TreeProgress => Set<TreeProgress>();
    public DbSet<StoryProgress> StoryProgress => Set<StoryProgress>();
    public DbSet<UserTokens> UserTokens => Set<UserTokens>();
    public DbSet<HeroProgress> HeroProgress => Set<HeroProgress>();
    public DbSet<HeroTreeProgress> HeroTreeProgress => Set<HeroTreeProgress>();
    
    // Tree of Light Model data
    public DbSet<TreeRegion> TreeRegions => Set<TreeRegion>();
    public DbSet<TreeStoryNode> TreeStoryNodes => Set<TreeStoryNode>();
    public DbSet<TreeUnlockRule> TreeUnlockRules => Set<TreeUnlockRule>();
    
    // Story System data
    public DbSet<StoryDefinition> StoryDefinitions => Set<StoryDefinition>();
    public DbSet<StoryTile> StoryTiles => Set<StoryTile>();
    public DbSet<StoryAnswer> StoryAnswers => Set<StoryAnswer>();
    public DbSet<UserStoryReadProgress> UserStoryReadProgress => Set<UserStoryReadProgress>();

    // Builder data
    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<Animal> Animals => Set<Animal>();
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
        });

        modelBuilder.Entity<CreditWallet>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne().HasForeignKey<CreditWallet>(x => x.UserId);
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

        // Seed builder data based on specs.txt
        // Parts
        var part_head = new BodyPart { Key = "head", Name = "Head", Image = "images/bodyparts/face.webp", IsBaseLocked = false };
        var part_body = new BodyPart { Key = "body", Name = "Body", Image = "images/bodyparts/body.webp", IsBaseLocked = false };
        var part_arms = new BodyPart { Key = "arms", Name = "Arms", Image = "images/bodyparts/hands.webp", IsBaseLocked = false };
        var part_legs = new BodyPart { Key = "legs", Name = "Legs", Image = "images/bodyparts/legs.webp", IsBaseLocked = true };
        var part_tail = new BodyPart { Key = "tail", Name = "Tail", Image = "images/bodyparts/tail.webp", IsBaseLocked = true };
        var part_wings = new BodyPart { Key = "wings", Name = "Wings", Image = "images/bodyparts/wings.webp", IsBaseLocked = true };
        var part_horn = new BodyPart { Key = "horn", Name = "Horn", Image = "images/bodyparts/horn.webp", IsBaseLocked = true };
        var part_horns = new BodyPart { Key = "horns", Name = "Horns", Image = "images/bodyparts/horns.webp", IsBaseLocked = true };

        modelBuilder.Entity<BodyPart>().HasData(part_head, part_body, part_arms, part_legs, part_tail, part_wings, part_horn, part_horns);

        // Regions (EN only)
        var r_sahara    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Sahara" };
        var r_jungle    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Jungle" };
        var r_farm      = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Farm" };
        var r_savanna   = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Savanna" };
        var r_forest    = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Forest" };
        var r_wetlands  = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Wetlands" };
        var r_mountains = new Region { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Mountains" };

        modelBuilder.Entity<Region>().HasData(r_sahara, r_jungle, r_farm, r_savanna, r_forest, r_wetlands, r_mountains);

        // Animals (EN)
        var a_bunny    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Label = "Bunny",              Src = "images/animals/base/bunny.jpg",              RegionId = r_farm.Id,      IsHybrid = false };
        var a_cat      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Label = "Cat",                Src = "images/animals/base/cat.jpg",                RegionId = r_farm.Id,      IsHybrid = false };
        var a_giraffe  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Label = "Giraffe",            Src = "images/animals/base/giraffe.jpg",            RegionId = r_savanna.Id,   IsHybrid = false };
        var a_dog      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Label = "Dog",                Src = "images/animals/base/dog.jpg",                RegionId = r_farm.Id,      IsHybrid = false };
        var a_fox      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Label = "Fox",                Src = "images/animals/base/fox.jpg",                RegionId = r_forest.Id,    IsHybrid = false };
        var a_hippo    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Label = "Hippo",              Src = "images/animals/base/hippo.jpg",              RegionId = r_wetlands.Id,  IsHybrid = false };
        var a_monkey   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Label = "Monkey",             Src = "images/animals/base/monkey.jpg",             RegionId = r_jungle.Id,    IsHybrid = false };
        var a_camel    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Label = "Camel",              Src = "images/animals/base/camel.jpg",              RegionId = r_sahara.Id,    IsHybrid = false };
        var a_deer     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Label = "Deer",               Src = "images/animals/base/deer.jpg",               RegionId = r_forest.Id,    IsHybrid = false };
        var a_duck     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000a"), Label = "Duck",               Src = "images/animals/base/duck.jpg",               RegionId = r_wetlands.Id,  IsHybrid = false };
        var a_eagle    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000b"), Label = "Eagle",              Src = "images/animals/base/eagle.jpg",              RegionId = r_mountains.Id, IsHybrid = false };
        var a_elephant = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000c"), Label = "Elephant",           Src = "images/animals/base/elephant.jpg",           RegionId = r_savanna.Id,   IsHybrid = false };
        var a_ostrich  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000d"), Label = "Ostrich",            Src = "images/animals/base/ostrich.jpg",            RegionId = r_savanna.Id,   IsHybrid = false };
        var a_parrot   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000e"), Label = "Parrot",             Src = "images/animals/base/parrot.jpg",             RegionId = r_jungle.Id,    IsHybrid = false };

        // Previously-RO animals converted to EN and mapped to EN regions
        var ar_jaguar   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000f"), Label = "Jaguar",             Src = "images/animals/base/jaguar.jpg",             RegionId = r_jungle.Id,    IsHybrid = false };
        var ar_toucan   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Label = "Toucan",             Src = "images/animals/base/toucan.jpg",             RegionId = r_jungle.Id,    IsHybrid = false };
        var ar_anaconda = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Label = "Anaconda",           Src = "images/animals/base/anaconda.jpg",           RegionId = r_jungle.Id,    IsHybrid = false };
        var ar_capuchin = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Label = "Capuchin Monkey",    Src = "images/animals/base/capuchin_monkey.jpg",    RegionId = r_jungle.Id,    IsHybrid = false };
        var ar_poisfrog = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Label = "Poison Dart Frog",   Src = "images/animals/base/poison_dart_frog.jpg",   RegionId = r_jungle.Id,    IsHybrid = false };

        var ar_lion     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Label = "Lion",               Src = "images/animals/base/lion.jpg",               RegionId = r_savanna.Id,   IsHybrid = false };
        var ar_afrele   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Label = "African Elephant",   Src = "images/animals/base/african_elephant.jpg",   RegionId = r_savanna.Id,   IsHybrid = false };
        var ar_giraffe  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), Label = "Giraffe",            Src = "images/animals/base/giraffe.jpg",            RegionId = r_savanna.Id,   IsHybrid = false };
        var ar_zebra    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), Label = "Zebra",              Src = "images/animals/base/zebra.jpg",              RegionId = r_savanna.Id,   IsHybrid = false };
        var ar_rhino    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), Label = "Rhinoceros",         Src = "images/animals/base/rhinoceros.jpg",         RegionId = r_savanna.Id,   IsHybrid = false };

        var ar_bison    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), Label = "Bison",              Src = "images/animals/base/bison.jpg",              RegionId = r_mountains.Id, IsHybrid = false };
        var ar_saiga    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001a"), Label = "Saiga Antelope",     Src = "images/animals/base/saiga_antelope.jpg",     RegionId = r_savanna.Id,   IsHybrid = false };
        var ar_graywolf = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001b"), Label = "Gray Wolf",          Src = "images/animals/base/gray_wolf.jpg",          RegionId = r_forest.Id,    IsHybrid = false };
        var ar_przew    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001c"), Label = "Przewalski's Horse", Src = "images/animals/base/przewalski_horse.jpg",  RegionId = r_mountains.Id, IsHybrid = false };
        var ar_steppeag = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001d"), Label = "Steppe Eagle",       Src = "images/animals/base/steppe_eagle.jpg",       RegionId = r_mountains.Id, IsHybrid = false };

        var ar_cow      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001e"), Label = "Cow",                Src = "images/animals/base/cow.jpg",                RegionId = r_farm.Id,      IsHybrid = false };
        var ar_sheep    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000001f"), Label = "Sheep",              Src = "images/animals/base/sheep.jpg",              RegionId = r_farm.Id,      IsHybrid = false };
        var ar_horse    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Label = "Horse",              Src = "images/animals/base/horse.jpg",              RegionId = r_farm.Id,      IsHybrid = false };
        var ar_chicken  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Label = "Chicken",            Src = "images/animals/base/chicken.jpg",            RegionId = r_farm.Id,      IsHybrid = false };
        var ar_pig      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Label = "Pig",                Src = "images/animals/base/pig.jpg",                RegionId = r_farm.Id,      IsHybrid = false };

        modelBuilder.Entity<Animal>().HasData(
            a_bunny, a_cat, a_giraffe, a_dog, a_fox, a_hippo, a_monkey, a_camel, a_deer, a_duck, a_eagle, a_elephant, a_ostrich, a_parrot,
            ar_jaguar, ar_toucan, ar_anaconda, ar_capuchin, ar_poisfrog,
            ar_lion, ar_afrele, ar_giraffe, ar_zebra, ar_rhino,
            ar_bison, ar_saiga, ar_graywolf, ar_przew, ar_steppeag,
            ar_cow, ar_sheep, ar_horse, ar_chicken, ar_pig
        );

        // Helper to create supports
        AnimalPartSupport APS(Guid animal, string part) => new AnimalPartSupport { AnimalId = animal, PartKey = part };

        var baseParts = new[] { "head", "body", "arms" };

        var supports = new List<AnimalPartSupport>();
        void AddSupports(Guid animal, IEnumerable<string> parts)
        {
            foreach (var p in parts)
                supports.Add(APS(animal, p));
        }

        // existing EN
        AddSupports(a_bunny.Id, baseParts);
        AddSupports(a_cat.Id, baseParts);
        AddSupports(a_giraffe.Id, new[] { "head","body","arms","legs","tail","wings","horn","horns" });
        AddSupports(a_dog.Id, baseParts);
        AddSupports(a_fox.Id, baseParts);
        AddSupports(a_hippo.Id, baseParts);
        AddSupports(a_monkey.Id, baseParts);
        AddSupports(a_camel.Id, baseParts);
        AddSupports(a_deer.Id, new[] { "head","body","arms","legs","tail","wings","horn","horns" });
        AddSupports(a_duck.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_eagle.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_elephant.Id, baseParts);
        AddSupports(a_ostrich.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(a_parrot.Id, new[] { "head","body","arms","legs","tail","wings" });

        // Converted animals supports (same as before)
        // Jungle
        AddSupports(ar_jaguar.Id, baseParts);
        AddSupports(ar_toucan.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(ar_anaconda.Id, new[] { "head","body","tail" });
        AddSupports(ar_capuchin.Id, baseParts);
        AddSupports(ar_poisfrog.Id, new[] { "head","body","legs","tail" });

        // Savanna
        AddSupports(ar_lion.Id, baseParts);
        AddSupports(ar_afrele.Id, baseParts);
        AddSupports(ar_giraffe.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_zebra.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_rhino.Id, new[] { "head","body","arms","legs","tail","horn" });

        // Mountains/Steppe-like
        AddSupports(ar_bison.Id, baseParts);
        AddSupports(ar_saiga.Id, new[] { "head","body","arms","legs","tail","horns" });
        AddSupports(ar_graywolf.Id, baseParts);
        AddSupports(ar_przew.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_steppeag.Id, new[] { "head","body","arms","legs","tail","wings" });

        // Farm
        AddSupports(ar_cow.Id, baseParts);
        AddSupports(ar_sheep.Id, baseParts);
        AddSupports(ar_horse.Id, new[] { "head","body","arms","legs","tail" });
        AddSupports(ar_chicken.Id, new[] { "head","body","arms","legs","tail","wings" });
        AddSupports(ar_pig.Id, baseParts);

        modelBuilder.Entity<AnimalPartSupport>().HasData(supports);

        // Tree of Light configurations
        modelBuilder.Entity<TreeProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.RegionId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<StoryProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<UserTokens>(e =>
        {
            e.HasKey(x => x.UserId);
            e.HasOne(x => x.User).WithOne().HasForeignKey<UserTokens>(x => x.UserId);
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

        // Tree of Light Model configurations
        modelBuilder.Entity<TreeRegion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(50);
            e.Property(x => x.Label).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Id).IsUnique();
        });

        modelBuilder.Entity<TreeStoryNode>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
            e.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.StoryId, x.RegionId }).IsUnique();
            e.HasOne(x => x.Region).WithMany(x => x.Stories).HasForeignKey(x => x.RegionId);
            e.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId);
        });

        modelBuilder.Entity<TreeUnlockRule>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Type).HasMaxLength(20).IsRequired();
            e.Property(x => x.FromId).HasMaxLength(100).IsRequired();
            e.Property(x => x.ToRegionId).HasMaxLength(50).IsRequired();
            // Note: No FK constraints to allow flexible FromId (can be region or story ID)
        });

        // Story System configurations
        modelBuilder.Entity<StoryDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => x.StoryId).IsUnique();
            e.HasMany(x => x.Tiles).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryTile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryDefinitionId, x.TileId }).IsUnique();
            e.HasOne(x => x.StoryDefinition).WithMany(x => x.Tiles).HasForeignKey(x => x.StoryDefinitionId);
            e.HasMany(x => x.Answers).WithOne(x => x.StoryTile).HasForeignKey(x => x.StoryTileId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StoryAnswer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.StoryTileId, x.AnswerId }).IsUnique();
            e.HasOne(x => x.StoryTile).WithMany(x => x.Answers).HasForeignKey(x => x.StoryTileId);
        });

        modelBuilder.Entity<UserStoryReadProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.UserId, x.StoryId, x.TileId }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        // Seed test user
        var testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<UserAlchimalia>().HasData(new UserAlchimalia
        {
            Id = testUserId,
            Auth0Sub = "test-user-sub",
            DisplayName = "Test User",
            CreatedAt = DateTime.UtcNow
        });

        // Config
        modelBuilder.Entity<BuilderConfig>().HasData(new BuilderConfig { Id = 1, BaseUnlockedAnimalCount = 3 });

        // Note: Tree of Light Model seeding moved to TreeOfLightService.SeedTreeModel()
        // for better control and avoiding FK constraint issues during migrations

        base.OnModelCreating(modelBuilder);
    }
}
