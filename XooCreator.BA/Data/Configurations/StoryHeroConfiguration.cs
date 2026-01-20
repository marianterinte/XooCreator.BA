using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryHeroConfiguration : IEntityTypeConfiguration<StoryHero>
{
    public void Configure(EntityTypeBuilder<StoryHero> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.UnlockConditionsJson).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired().HasDefaultValue("draft");
        builder.Property(x => x.Version).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.ReviewNotes).HasMaxLength(2000);
        builder.HasIndex(x => x.HeroId).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Id, x.Status });
    }
}

public class StoryHeroTranslationConfiguration : IEntityTypeConfiguration<StoryHeroTranslation>
{
    public void Configure(EntityTypeBuilder<StoryHeroTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.GreetingAudioUrl).HasMaxLength(500);
        builder.HasIndex(x => new { x.StoryHeroId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryHero)
            .WithMany(s => s.Translations)
            .HasForeignKey(x => x.StoryHeroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryHeroVersionConfiguration : IEntityTypeConfiguration<StoryHeroVersion>
{
    public void Configure(EntityTypeBuilder<StoryHeroVersion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.StoryHeroId, x.Version }).IsUnique();
        builder.HasOne(x => x.StoryHero)
            .WithMany()
            .HasForeignKey(x => x.StoryHeroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
