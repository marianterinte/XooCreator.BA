using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class SupporterPackPlanExclusiveConfiguration : IEntityTypeConfiguration<SupporterPackPlanExclusive>
{
    public void Configure(EntityTypeBuilder<SupporterPackPlanExclusive> builder)
    {
        builder.ToTable("SupporterPackPlanExclusives", "alchimalia_schema");
        builder.HasKey(x => x.PlanId);
        builder.Property(x => x.PlanId).HasMaxLength(50);
        builder.Property(x => x.ExclusiveStoryIdsJson).HasMaxLength(4000);
        builder.Property(x => x.ExclusiveEpicIdsJson).HasMaxLength(4000);
    }
}
