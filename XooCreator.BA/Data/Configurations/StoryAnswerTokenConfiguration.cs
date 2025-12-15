using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAnswerTokenConfiguration : IEntityTypeConfiguration<StoryAnswerToken>
{
    public void Configure(EntityTypeBuilder<StoryAnswerToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(128).IsRequired();
    }
}
