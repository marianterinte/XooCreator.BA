using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicReaderConfiguration : IEntityTypeConfiguration<EpicReader>
{
    public void Configure(EntityTypeBuilder<EpicReader> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.AcquiredAt).IsRequired();
        builder.Property(x => x.AcquisitionSource).HasConversion<int>();
        builder.HasIndex(x => x.EpicId);
        builder.HasIndex(x => new { x.UserId, x.EpicId }).IsUnique();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Epic)
            .WithMany()
            .HasForeignKey(x => x.EpicId)
            .HasPrincipalKey(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
