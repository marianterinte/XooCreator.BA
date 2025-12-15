using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEvaluationResultConfiguration : IEntityTypeConfiguration<StoryEvaluationResult>
{
    public void Configure(EntityTypeBuilder<StoryEvaluationResult> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.StoryId, x.CompletedAt });
        builder.HasIndex(x => new { x.UserId, x.StoryId, x.SessionId }).IsUnique();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
