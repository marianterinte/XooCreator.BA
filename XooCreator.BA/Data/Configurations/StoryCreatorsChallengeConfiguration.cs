using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeConfiguration : IEntityTypeConfiguration<StoryCreatorsChallenge>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallenge> builder)
    {
        builder.ToTable("StoryCreatorsChallenges", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ChallengeId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired().HasDefaultValue("active");
        builder.Property(x => x.SortOrder).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.EndDate); // Optional
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.SortOrder);
        builder.HasIndex(x => x.EndDate);
        builder.HasIndex(x => x.ChallengeId).IsUnique();
        
        builder.HasMany(x => x.Translations)
            .WithOne(x => x.Challenge)
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Challenge)
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
