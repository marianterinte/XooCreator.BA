using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionDefinitionTranslationConfiguration : IEntityTypeConfiguration<StoryRegionDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<StoryRegionDefinitionTranslation> builder)
    {
        builder.ToTable("StoryRegionDefinitionTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryRegionDefinitionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description);
        builder.HasIndex(x => new { x.StoryRegionDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.StoryRegionDefinitionId);
        builder.HasOne(x => x.StoryRegionDefinition).WithMany(x => x.Translations).HasForeignKey(x => x.StoryRegionDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}

