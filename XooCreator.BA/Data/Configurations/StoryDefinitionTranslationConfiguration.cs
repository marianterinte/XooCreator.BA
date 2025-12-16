using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryDefinitionTranslationConfiguration : IEntityTypeConfiguration<StoryDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<StoryDefinitionTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryDefinition)
            .WithMany(s => s.Translations)
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
