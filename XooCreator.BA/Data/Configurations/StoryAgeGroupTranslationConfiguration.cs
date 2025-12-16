using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAgeGroupTranslationConfiguration : IEntityTypeConfiguration<StoryAgeGroupTranslation>
{
    public void Configure(EntityTypeBuilder<StoryAgeGroupTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryAgeGroupId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryAgeGroup)
            .WithMany(t => t.Translations)
            .HasForeignKey(x => x.StoryAgeGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
