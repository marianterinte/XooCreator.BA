using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserGeneratedLoiAnimalConfiguration : IEntityTypeConfiguration<UserGeneratedLoiAnimal>
{
    public void Configure(EntityTypeBuilder<UserGeneratedLoiAnimal> builder)
    {
        builder.ToTable("UserGeneratedLoiAnimals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ImageBlobPath).IsRequired();
        builder.Property(x => x.StoryText).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
