using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeOfHeroesConfigDefinitionNodeConfiguration : IEntityTypeConfiguration<TreeOfHeroesConfigDefinitionNode>
{
    public void Configure(EntityTypeBuilder<TreeOfHeroesConfigDefinitionNode> builder)
    {
        builder.ToTable("TreeOfHeroesConfigDefinitionNodes", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HeroDefinitionId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.ConfigDefinitionId, x.HeroDefinitionId }).IsUnique();
        builder.HasOne(x => x.Config).WithMany(x => x.Nodes).HasForeignKey(x => x.ConfigDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.HeroDefinition).WithMany().HasForeignKey(x => x.HeroDefinitionId).OnDelete(DeleteBehavior.Restrict);
    }
}
