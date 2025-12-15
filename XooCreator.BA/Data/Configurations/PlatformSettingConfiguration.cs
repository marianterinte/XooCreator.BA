using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class PlatformSettingConfiguration : IEntityTypeConfiguration<PlatformSetting>
{
    public void Configure(EntityTypeBuilder<PlatformSetting> builder)
    {
        // Schema will be set by default schema in OnModelCreating
        builder.ToTable("PlatformSettings");
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(128).IsRequired();
        builder.Property(x => x.BoolValue).IsRequired();
        builder.Property(x => x.StringValue);
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.UpdatedBy).HasMaxLength(256);
    }
}
