using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeTranslationConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeTranslation> builder)
    {
        builder.ToTable("StoryCreatorsChallengeTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ChallengeId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Topic).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description); // Optional
        
        builder.HasIndex(x => new { x.ChallengeId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.ChallengeId);
        builder.HasIndex(x => x.LanguageCode);
        
        builder.HasOne(x => x.Challenge)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
