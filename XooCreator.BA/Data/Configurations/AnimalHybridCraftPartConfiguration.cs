using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalHybridCraftPartConfiguration : IEntityTypeConfiguration<AnimalHybridCraftPart>
{
    public void Configure(EntityTypeBuilder<AnimalHybridCraftPart> builder)
    {
        builder.ToTable("AnimalHybridCraftParts", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BodyPartKey).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.AnimalCraftId);
        builder.HasOne(x => x.AnimalCraft).WithMany(x => x.HybridParts).HasForeignKey(x => x.AnimalCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SourceAnimal).WithMany().HasForeignKey(x => x.SourceAnimalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BodyPart).WithMany().HasForeignKey(x => x.BodyPartKey).OnDelete(DeleteBehavior.Restrict);
    }
}
