using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalCraftPartSupportConfiguration : IEntityTypeConfiguration<AnimalCraftPartSupport>
{
    public void Configure(EntityTypeBuilder<AnimalCraftPartSupport> builder)
    {
        builder.ToTable("AnimalCraftPartSupports", "alchimalia_schema");
        builder.HasKey(x => new { x.AnimalCraftId, x.BodyPartKey });
        builder.Property(x => x.BodyPartKey).HasMaxLength(50).IsRequired();
        builder.HasOne(x => x.AnimalCraft).WithMany(x => x.SupportedParts).HasForeignKey(x => x.AnimalCraftId);
        builder.HasOne(x => x.BodyPart).WithMany().HasForeignKey(x => x.BodyPartKey);
    }
}
