using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeItemRewardConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeItemReward>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeItemReward> builder)
    {
        builder.ToTable("StoryCreatorsChallengeItemRewards", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ItemId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TokenType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TokenValue).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Quantity).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.SortOrder).IsRequired().HasDefaultValue(0);
        
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.TokenType);
        
        builder.HasOne(x => x.Item)
            .WithMany(x => x.Rewards)
            .HasForeignKey(x => x.ItemId)
            .HasPrincipalKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
