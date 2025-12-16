using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftAnswerTokenConfiguration : IEntityTypeConfiguration<StoryCraftAnswerToken>
{
    public void Configure(EntityTypeBuilder<StoryCraftAnswerToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(128).IsRequired();
        builder.HasOne(x => x.StoryCraftAnswer).WithMany(x => x.Tokens).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
    }
}
