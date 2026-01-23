using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeSubmissionConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeSubmission>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeSubmission> builder)
    {
        builder.ToTable("StoryCreatorsChallengeSubmissions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ChallengeId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.SubmittedAt).IsRequired();
        builder.Property(x => x.LikesCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsWinner).IsRequired().HasDefaultValue(false);
        
        builder.HasIndex(x => new { x.ChallengeId, x.StoryId }).IsUnique();
        builder.HasIndex(x => x.ChallengeId);
        builder.HasIndex(x => x.StoryId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LikesCount);
        
        builder.HasOne(x => x.Challenge)
            .WithMany()
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
