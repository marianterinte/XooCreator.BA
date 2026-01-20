using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalVersionConfiguration : IEntityTypeConfiguration<AnimalVersion>
{
    public void Configure(EntityTypeBuilder<AnimalVersion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.AnimalId, x.Version }).IsUnique();
        builder.HasOne(x => x.Animal)
            .WithMany()
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
