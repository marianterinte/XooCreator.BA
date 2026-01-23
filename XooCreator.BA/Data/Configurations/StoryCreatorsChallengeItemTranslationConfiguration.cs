using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeItemTranslationConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeItemTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeItemTranslation> builder)
    {
        builder.ToTable("StoryCreatorsChallengeItemTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ItemId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description); // Optional
        
        builder.HasIndex(x => new { x.ItemId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.LanguageCode);
        
        builder.HasOne(x => x.Item)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ItemId)
            .HasPrincipalKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
