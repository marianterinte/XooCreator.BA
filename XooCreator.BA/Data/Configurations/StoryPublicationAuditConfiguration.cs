using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryPublicationAuditConfiguration : IEntityTypeConfiguration<StoryPublicationAudit>
{
    public void Configure(EntityTypeBuilder<StoryPublicationAudit> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PerformedByEmail).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Action).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => x.StoryId);
        builder.HasIndex(x => x.StoryDefinitionId);
        builder.HasOne(x => x.StoryDefinition)
            .WithMany()
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
