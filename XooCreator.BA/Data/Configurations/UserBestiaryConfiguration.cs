using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserBestiaryConfiguration : IEntityTypeConfiguration<UserBestiary>
{
    public void Configure(EntityTypeBuilder<UserBestiary> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.BestiaryType).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.BestiaryItemId, x.BestiaryType }).IsUnique();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.BestiaryItem).WithMany().HasForeignKey(x => x.BestiaryItemId);
        builder.ToTable("UserBestiary");
    }
}
