using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroCraftConfiguration : IEntityTypeConfiguration<EpicHeroCraft>
{
    public void Configure(EntityTypeBuilder<EpicHeroCraft> builder)
    {
        builder.ToTable("EpicHeroCrafts", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => new { x.OwnerUserId, x.Id }).IsUnique();
        builder.HasIndex(x => new { x.Id, x.Status });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.AssignedReviewerUserId).HasFilter($"[{nameof(EpicHeroCraft.AssignedReviewerUserId)}] IS NOT NULL");
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Cascade);
        // Review workflow foreign keys (optional, nullable)
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.AssignedReviewerUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

