using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftHeroReferenceConfiguration : IEntityTypeConfiguration<StoryEpicCraftHeroReference>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftHeroReference> builder)
    {
        builder.ToTable("StoryEpicCraftHeroReferences", "alchimalia_schema");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.EpicId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.HeroId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.StoryId)
            .HasMaxLength(200);
        
        builder.HasIndex(x => x.EpicId);
        builder.HasIndex(x => new { x.EpicId, x.HeroId }).IsUnique();
        builder.HasIndex(x => x.HeroId);
        
        // Foreign key to StoryEpicCraft
        builder.HasOne(x => x.Epic)
            .WithMany(e => e.HeroReferences)
            .HasForeignKey(x => x.EpicId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Foreign key to EpicHeroCraft (for draft epics)
        builder.HasOne(x => x.Hero)
            .WithMany()
            .HasForeignKey(x => x.HeroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

