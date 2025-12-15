using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroClickMessageConfiguration : IEntityTypeConfiguration<HeroClickMessage>
{
    public void Configure(EntityTypeBuilder<HeroClickMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MessageKey).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.HeroId).IsUnique();
    }
}
