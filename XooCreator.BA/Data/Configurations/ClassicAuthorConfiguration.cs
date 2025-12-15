using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class ClassicAuthorConfiguration : IEntityTypeConfiguration<ClassicAuthor>
{
    public void Configure(EntityTypeBuilder<ClassicAuthor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.AuthorId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => x.AuthorId).IsUnique();
        builder.HasIndex(x => new { x.LanguageCode, x.SortOrder });
    }
}
