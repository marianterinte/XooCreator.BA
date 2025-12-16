using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class BestiaryItemConfiguration : IEntityTypeConfiguration<BestiaryItem>
{
    public void Configure(EntityTypeBuilder<BestiaryItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ArmsKey).HasMaxLength(32);
        builder.Property(x => x.BodyKey).HasMaxLength(32);
        builder.Property(x => x.HeadKey).HasMaxLength(32);
        builder.Property(x => x.Name).HasMaxLength(128);
        builder.Property(x => x.Story).HasMaxLength(10000);
    }
}
