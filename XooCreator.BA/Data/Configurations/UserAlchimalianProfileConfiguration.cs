using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Configurations;

public class UserAlchimalianProfileConfiguration : IEntityTypeConfiguration<UserAlchimalianProfile>
{
    public void Configure(EntityTypeBuilder<UserAlchimalianProfile> builder)
    {
        builder.ToTable("UserAlchimalianProfiles", "alchimalia_schema");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.SelectedHeroId).HasMaxLength(100);
        builder.Property(x => x.DiscoveredHeroIdsJson).HasColumnName("DiscoveredHeroIds").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
