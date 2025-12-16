using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryReaderConfiguration : IEntityTypeConfiguration<StoryReader>
{
    public void Configure(EntityTypeBuilder<StoryReader> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.AcquiredAt).IsRequired();
        builder.Property(x => x.AcquisitionSource).HasConversion<int>();
        builder.HasIndex(x => x.StoryId);
        builder.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
