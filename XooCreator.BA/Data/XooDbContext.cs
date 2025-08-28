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
