using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAnswerTranslationConfiguration : IEntityTypeConfiguration<StoryAnswerTranslation>
{
    public void Configure(EntityTypeBuilder<StoryAnswerTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryAnswerId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryAnswer)
            .WithMany(a => a.Translations)
            .HasForeignKey(x => x.StoryAnswerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
