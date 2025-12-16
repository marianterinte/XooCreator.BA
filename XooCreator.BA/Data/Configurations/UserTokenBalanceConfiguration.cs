using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserTokenBalanceConfiguration : IEntityTypeConfiguration<UserTokenBalance>
{
    public void Configure(EntityTypeBuilder<UserTokenBalance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.Type, x.Value }).IsUnique();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}
