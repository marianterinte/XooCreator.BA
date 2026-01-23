using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCreatorsChallengeSubscriptionConfiguration : IEntityTypeConfiguration<StoryCreatorsChallengeSubscription>
{
    public void Configure(EntityTypeBuilder<StoryCreatorsChallengeSubscription> builder)
    {
        builder.ToTable("StoryCreatorsChallengeSubscriptions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ChallengeId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.SubscribedAt).IsRequired();
        
        builder.HasIndex(x => new { x.ChallengeId, x.UserId }).IsUnique();
        builder.HasIndex(x => x.ChallengeId);
        builder.HasIndex(x => x.UserId);
        
        builder.HasOne(x => x.Challenge)
            .WithMany()
            .HasForeignKey(x => x.ChallengeId)
            .HasPrincipalKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Assuming we can map to User, but if not we might need to be careful.
        // The migration V0079 references "alchimalia_schema"."AlchimaliaUsers".
        // Use generic WithMany() if User navigation is not bi-directional.
        // If AlchimaliaUser entity is not available in this context, we might skip navigation property configuration 
        // but looking at implementation guide it suggests: public AlchimaliaUser User { get; set; }
    }
}
