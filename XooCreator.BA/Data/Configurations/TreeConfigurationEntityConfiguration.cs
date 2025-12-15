using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XooCreator.BA.Data.Configurations;

public class TreeConfigurationEntityConfiguration : IEntityTypeConfiguration<XooCreator.BA.Data.TreeConfiguration>
{
    public void Configure(EntityTypeBuilder<XooCreator.BA.Data.TreeConfiguration> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
    }
}
