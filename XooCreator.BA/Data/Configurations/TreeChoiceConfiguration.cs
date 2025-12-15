using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeChoiceConfiguration : IEntityTypeConfiguration<TreeChoice>
{
    public void Configure(EntityTypeBuilder<TreeChoice> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Tree).WithMany(x => x.Choices).HasForeignKey(x => x.TreeId);
        builder.HasIndex(x => new { x.TreeId, x.Tier }).IsUnique();
    }
}
