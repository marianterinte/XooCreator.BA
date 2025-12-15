using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class BuilderConfigConfiguration : IEntityTypeConfiguration<BuilderConfig>
{
    public void Configure(EntityTypeBuilder<BuilderConfig> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
