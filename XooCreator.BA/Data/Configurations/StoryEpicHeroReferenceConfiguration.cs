using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicHeroReferenceConfiguration : IEntityTypeConfiguration<StoryEpicHeroReference>
{
    public void Configure(EntityTypeBuilder<StoryEpicHeroReference> builder)
    {
        builder.ToTable("StoryEpicHeroReferences", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StoryId).HasMaxLength(200);
        builder.HasIndex(x => new { x.EpicId, x.HeroId }).IsUnique();
        builder.HasOne(x => x.Epic).WithMany().HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
        // Foreign key to EpicHeroDefinition (for published epics)
        builder.HasOne(x => x.Hero)
            .WithMany()
            .HasForeignKey(x => x.HeroId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if used in epics
    }
}
