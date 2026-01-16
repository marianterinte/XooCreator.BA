using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalDefinitionConfiguration : IEntityTypeConfiguration<AnimalDefinition>
{
    public void Configure(EntityTypeBuilder<AnimalDefinition> builder)
    {
        builder.ToTable("AnimalDefinitions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Label).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Src).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.RegionId);
        builder.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.PublishedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
