using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class WelcomeFlowConfigConfiguration : IEntityTypeConfiguration<WelcomeFlowConfig>
{
    public void Configure(EntityTypeBuilder<WelcomeFlowConfig> builder)
    {
        builder.ToTable("WelcomeFlowConfig", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryPointStoryId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.KindergartenGirl).HasMaxLength(128).IsRequired();
        builder.Property(x => x.KindergartenBoy).HasMaxLength(128).IsRequired();
        builder.Property(x => x.PrimaryGirl).HasMaxLength(128).IsRequired();
        builder.Property(x => x.PrimaryBoy).HasMaxLength(128).IsRequired();
        builder.Property(x => x.OlderGirl).HasMaxLength(128).IsRequired();
        builder.Property(x => x.OlderBoy).HasMaxLength(128).IsRequired();
    }
}
