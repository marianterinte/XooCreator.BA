using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeOfHeroesConfigCraftConfiguration : IEntityTypeConfiguration<TreeOfHeroesConfigCraft>
{
    public void Configure(EntityTypeBuilder<TreeOfHeroesConfigCraft> builder)
    {
        builder.ToTable("TreeOfHeroesConfigCrafts", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Label).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Status);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
