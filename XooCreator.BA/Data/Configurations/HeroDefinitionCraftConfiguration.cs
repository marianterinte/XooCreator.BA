using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionCraftConfiguration : IEntityTypeConfiguration<HeroDefinitionCraft>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionCraft> builder)
    {
        builder.ToTable("HeroDefinitionCrafts", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PublishedDefinitionId).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Image).HasMaxLength(500);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedByUserId).HasFilter($"[{nameof(HeroDefinitionCraft.CreatedByUserId)}] IS NOT NULL");
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
