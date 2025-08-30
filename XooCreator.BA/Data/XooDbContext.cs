using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Data;

public class XooDbContext : DbContext
{
    public XooDbContext(DbContextOptions<XooDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CreditWallet> CreditWallets => Set<CreditWallet>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<TreeChoice> TreeChoices => Set<TreeChoice>();
    public DbSet<Creature> Creatures => Set<Creature>();
    public DbSet<Job> Jobs => Set<Job>();

    // Builder data
    public DbSet<BodyPart> BodyParts => Set<BodyPart>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalPartSupport> AnimalPartSupports => Set<AnimalPartSupport>();
    public DbSet<BuilderConfig> BuilderConfigs => Set<BuilderConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<User>(e =>
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

        modelBuilder.Entity<Animal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(64).IsRequired();
            e.Property(x => x.Src).HasMaxLength(256).IsRequired();
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

        // Animals with valid deterministic GUIDs
        var a_bunny    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Label = "Bunny",    Src = "images/animals/base/bunny.jpg" };
        var a_cat      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Label = "Cat",      Src = "images/animals/base/cat.jpg" };
        var a_giraffe  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Label = "Giraffe",  Src = "images/animals/base/giraffe.jpg" };
        var a_dog      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Label = "Dog",      Src = "images/animals/base/dog.jpg" };
        var a_fox      = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Label = "Fox",      Src = "images/animals/base/fox.jpg" };
        var a_hippo    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Label = "Hippo",    Src = "images/animals/base/hippo.jpg" };
        var a_monkey   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Label = "Monkey",   Src = "images/animals/base/monkey.jpg" };
        var a_camel    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Label = "Camel",    Src = "images/animals/base/camel.jpg" };
        var a_deer     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Label = "Deer",     Src = "images/animals/base/deer.jpg" };
        var a_duck     = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000a"), Label = "Duck",     Src = "images/animals/base/duck.jpg" };
        var a_eagle    = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000b"), Label = "Eagle",    Src = "images/animals/base/eagle.jpg" };
        var a_elephant = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000c"), Label = "Elephant", Src = "images/animals/base/elephant.jpg" };
        var a_ostrich  = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000d"), Label = "Ostrich",  Src = "images/animals/base/ostrich.jpg" };
        var a_parrot   = new Animal { Id = Guid.Parse("00000000-0000-0000-0000-00000000000e"), Label = "Parrot",   Src = "images/animals/base/parrot.jpg" };

        modelBuilder.Entity<Animal>().HasData(
            a_bunny, a_cat, a_giraffe, a_dog, a_fox, a_hippo, a_monkey, a_camel, a_deer, a_duck, a_eagle, a_elephant, a_ostrich, a_parrot
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

        modelBuilder.Entity<AnimalPartSupport>().HasData(supports);

        // Config
        modelBuilder.Entity<BuilderConfig>().HasData(new BuilderConfig { Id = 1, BaseUnlockedAnimalCount = 3 });

        base.OnModelCreating(modelBuilder);
    }
}

public class User
{
    public Guid Id { get; set; }
    public string Auth0Sub { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreditWallet
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int Balance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum CreditTransactionType
{
    Purchase,
    Spend,
    Grant
}

public class CreditTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int Amount { get; set; }
    public CreditTransactionType Type { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Tree
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string RootType { get; set; } = string.Empty; // color or type
    public string? Location { get; set; }
    public int CurrentTier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TreeChoice> Choices { get; set; } = new List<TreeChoice>();
}

public class TreeChoice
{
    public Guid Id { get; set; }
    public Guid TreeId { get; set; }
    public Tree Tree { get; set; } = null!;
    public int Tier { get; set; }
    public string BranchType { get; set; } = string.Empty; // thin|thick|balanced
    public string TraitAwarded { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Creature
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? TreeId { get; set; }
    public Tree? Tree { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Archetype { get; set; } = string.Empty;
    public string TraitsJson { get; set; } = "{}"; // jsonb
    public string Rarity { get; set; } = "common";
    public string? ImageUrl { get; set; }
    public string? ThumbUrl { get; set; }
    public string PromptUsedJson { get; set; } = "{}";
    public string? Story { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum JobType { Image, Story }
public enum JobStatus { Queued, Running, Done, Error }

public class Job
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public JobType Type { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public string? ResultUrl { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Builder entities
public class BodyPart
{
    public string Key { get; set; } = string.Empty; // e.g., head
    public string Name { get; set; } = string.Empty; // e.g., Head
    public string Image { get; set; } = string.Empty; // e.g., images/bodyparts/face.webp
    public bool IsBaseLocked { get; set; }
}

public class Animal
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty; // e.g., Bunny
    public string Src { get; set; } = string.Empty;   // image url

    public ICollection<AnimalPartSupport> SupportedParts { get; set; } = new List<AnimalPartSupport>();
}

public class AnimalPartSupport
{
    public Guid AnimalId { get; set; }
    public Animal Animal { get; set; } = null!;

    public string PartKey { get; set; } = string.Empty;
    public BodyPart Part { get; set; } = null!;
}

public class BuilderConfig
{
    public int Id { get; set; }
    public int BaseUnlockedAnimalCount { get; set; }
}
