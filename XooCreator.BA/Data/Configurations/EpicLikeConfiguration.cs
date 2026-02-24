using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Configurations;

public class EpicLikeConfiguration : IEntityTypeConfiguration<EpicLike>
{
    public void Configure(EntityTypeBuilder<EpicLike> builder)
    {
        builder.ToTable("EpicLikes", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LikedAt).IsRequired();
        builder.HasIndex(x => x.EpicId).HasDatabaseName("IX_EpicLikes_EpicId");
        builder.HasIndex(x => new { x.UserId, x.EpicId }).IsUnique().HasDatabaseName("IX_EpicLikes_UserId_EpicId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
