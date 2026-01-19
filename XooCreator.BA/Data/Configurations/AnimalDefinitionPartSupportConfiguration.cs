using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalDefinitionPartSupportConfiguration : IEntityTypeConfiguration<AnimalDefinitionPartSupport>
{
    public void Configure(EntityTypeBuilder<AnimalDefinitionPartSupport> builder)
    {
        builder.ToTable("AnimalDefinitionPartSupports", "alchimalia_schema");
        builder.HasKey(x => new { x.AnimalDefinitionId, x.BodyPartKey });
        builder.Property(x => x.BodyPartKey).HasMaxLength(50).IsRequired();
        builder.HasOne(x => x.AnimalDefinition).WithMany(x => x.SupportedParts).HasForeignKey(x => x.AnimalDefinitionId);
        builder.HasOne(x => x.BodyPart).WithMany().HasForeignKey(x => x.BodyPartKey);
    }
}
