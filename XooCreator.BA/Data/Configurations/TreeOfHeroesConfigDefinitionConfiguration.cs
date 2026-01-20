using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeOfHeroesConfigDefinitionConfiguration : IEntityTypeConfiguration<TreeOfHeroesConfigDefinition>
{
    public void Configure(EntityTypeBuilder<TreeOfHeroesConfigDefinition> builder)
    {
        builder.ToTable("TreeOfHeroesConfigDefinitions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Label).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Status);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.PublishedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
