using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class CreditWalletConfiguration : IEntityTypeConfiguration<CreditWallet>
{
    public void Configure(EntityTypeBuilder<CreditWallet> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasOne(x => x.User).WithOne().HasForeignKey<CreditWallet>(x => x.UserId);
    }
}
