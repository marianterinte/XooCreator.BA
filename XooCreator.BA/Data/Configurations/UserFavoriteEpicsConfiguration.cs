using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserFavoriteEpicsConfiguration : IEntityTypeConfiguration<UserFavoriteEpics>
{
    public void Configure(EntityTypeBuilder<UserFavoriteEpics> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.AddedAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.EpicId }).IsUnique();
        builder.HasIndex(x => x.EpicId);
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
