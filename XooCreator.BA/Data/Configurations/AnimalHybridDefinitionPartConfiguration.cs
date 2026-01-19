using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalHybridDefinitionPartConfiguration : IEntityTypeConfiguration<AnimalHybridDefinitionPart>
{
    public void Configure(EntityTypeBuilder<AnimalHybridDefinitionPart> builder)
    {
        builder.ToTable("AnimalHybridDefinitionParts", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BodyPartKey).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.AnimalDefinitionId);
        builder.HasOne(x => x.AnimalDefinition).WithMany(x => x.HybridParts).HasForeignKey(x => x.AnimalDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SourceAnimal).WithMany().HasForeignKey(x => x.SourceAnimalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BodyPart).WithMany().HasForeignKey(x => x.BodyPartKey).OnDelete(DeleteBehavior.Restrict);
    }
}
