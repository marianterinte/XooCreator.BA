using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryPrintRecordConfiguration : IEntityTypeConfiguration<StoryPrintRecord>
{
    public void Configure(EntityTypeBuilder<StoryPrintRecord> builder)
    {
        builder.ToTable("StoryPrintRecords", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(255).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.StoryId);
        builder.HasIndex(x => x.PrintedAtUtc);
    }
}
