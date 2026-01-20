using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeOfHeroesConfigCraftEdgeConfiguration : IEntityTypeConfiguration<TreeOfHeroesConfigCraftEdge>
{
    public void Configure(EntityTypeBuilder<TreeOfHeroesConfigCraftEdge> builder)
    {
        builder.ToTable("TreeOfHeroesConfigCraftEdges", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FromHeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ToHeroId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.ConfigCraftId, x.FromHeroId, x.ToHeroId }).IsUnique();
        builder.HasOne(x => x.Config).WithMany(x => x.Edges).HasForeignKey(x => x.ConfigCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FromHero).WithMany().HasForeignKey(x => x.FromHeroId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToHero).WithMany().HasForeignKey(x => x.ToHeroId).OnDelete(DeleteBehavior.Restrict);
    }
}
