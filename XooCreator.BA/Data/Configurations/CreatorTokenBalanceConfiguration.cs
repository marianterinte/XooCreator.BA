using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class CreatorTokenBalanceConfiguration : IEntityTypeConfiguration<CreatorTokenBalance>
{
    public void Configure(EntityTypeBuilder<CreatorTokenBalance> builder)
    {
        builder.ToTable("CreatorTokenBalances", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TokenType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TokenValue).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Quantity).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsAdminOverride).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.OverrideByUserId);
        builder.Property(x => x.OverrideAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        
        // Unique constraint: one balance per (UserId, TokenType, TokenValue)
        builder.HasIndex(x => new { x.UserId, x.TokenType, x.TokenValue })
            .IsUnique()
            .HasDatabaseName("UQ_CreatorTokenBalances_User_TokenType_TokenValue");
        
        // Indexes for queries
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TokenType);
        builder.HasIndex(x => new { x.UserId, x.TokenType });
        
        // Foreign keys
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.OverrideByUser)
            .WithMany()
            .HasForeignKey(x => x.OverrideByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
