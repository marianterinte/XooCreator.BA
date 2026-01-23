using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeItemConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeItem>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeItem> builder)
    {
        builder.ToTable("StoryCreatorsChallengeItems", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ChallengeId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ItemId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired().HasDefaultValue(0);
        
        builder.HasIndex(x => new { x.ChallengeId, x.ItemId }).IsUnique();
        builder.HasIndex(x => x.ItemId).IsUnique(); // Required for FK reference from Translations/Rewards
        builder.HasIndex(x => x.ChallengeId);
        builder.HasIndex(x => x.SortOrder);
        
        builder.HasOne(x => x.Challenge)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Translations)
            .WithOne(x => x.Item)
            .HasForeignKey(x => x.ItemId)
            .HasPrincipalKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Rewards)
            .WithOne(x => x.Item)
            .HasForeignKey(x => x.ItemId)
            .HasPrincipalKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
