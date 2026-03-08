using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class SupporterPackPlanConfiguration : IEntityTypeConfiguration<SupporterPackPlan>
{
    public void Configure(EntityTypeBuilder<SupporterPackPlan> builder)
    {
        builder.ToTable("SupporterPackPlans", "alchimalia_schema");
        builder.HasKey(x => x.PlanId);
        builder.Property(x => x.PlanId).HasMaxLength(50);
        builder.Property(x => x.PriceRon).HasPrecision(18, 2);
    }
}
