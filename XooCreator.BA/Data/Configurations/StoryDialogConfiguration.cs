using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftDialogParticipantConfiguration : IEntityTypeConfiguration<StoryCraftDialogParticipant>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogParticipant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftId, x.HeroId }).IsUnique();
        builder.HasOne(x => x.StoryCraft)
            .WithMany(x => x.DialogParticipants)
            .HasForeignKey(x => x.StoryCraftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryCraftDialogTileConfiguration : IEntityTypeConfiguration<StoryCraftDialogTile>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogTile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.StoryCraftTileId).IsUnique();

        builder.HasOne(x => x.StoryCraft)
            .WithMany()
            .HasForeignKey(x => x.StoryCraftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StoryCraftTile)
            .WithOne(x => x.DialogTile)
            .HasForeignKey<StoryCraftDialogTile>(x => x.StoryCraftTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryCraftDialogNodeConfiguration : IEntityTypeConfiguration<StoryCraftDialogNode>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftDialogTileId, x.NodeId }).IsUnique();
        builder.HasOne(x => x.StoryCraftDialogTile)
            .WithMany(x => x.Nodes)
            .HasForeignKey(x => x.StoryCraftDialogTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryCraftDialogNodeTranslationConfiguration : IEntityTypeConfiguration<StoryCraftDialogNodeTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogNodeTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftDialogNodeId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryCraftDialogNode)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.StoryCraftDialogNodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryCraftDialogEdgeConfiguration : IEntityTypeConfiguration<StoryCraftDialogEdge>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogEdge> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftDialogNodeId, x.EdgeId }).IsUnique();
        builder.HasOne(x => x.StoryCraftDialogNode)
            .WithMany(x => x.OutgoingEdges)
            .HasForeignKey(x => x.StoryCraftDialogNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.JumpToTileId).HasMaxLength(100);
        builder.Property(x => x.SetBranchId).HasMaxLength(100);
    }
}

public class StoryCraftDialogEdgeTranslationConfiguration : IEntityTypeConfiguration<StoryCraftDialogEdgeTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogEdgeTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftDialogEdgeId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryCraftDialogEdge)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.StoryCraftDialogEdgeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogParticipantConfiguration : IEntityTypeConfiguration<StoryDialogParticipant>
{
    public void Configure(EntityTypeBuilder<StoryDialogParticipant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryDefinitionId, x.HeroId }).IsUnique();
        builder.HasOne(x => x.StoryDefinition)
            .WithMany(x => x.DialogParticipants)
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogTileConfiguration : IEntityTypeConfiguration<StoryDialogTile>
{
    public void Configure(EntityTypeBuilder<StoryDialogTile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.StoryTileId).IsUnique();

        builder.HasOne(x => x.StoryDefinition)
            .WithMany()
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StoryTile)
            .WithOne(x => x.DialogTile)
            .HasForeignKey<StoryDialogTile>(x => x.StoryTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogNodeConfiguration : IEntityTypeConfiguration<StoryDialogNode>
{
    public void Configure(EntityTypeBuilder<StoryDialogNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryDialogTileId, x.NodeId }).IsUnique();
        builder.HasOne(x => x.StoryDialogTile)
            .WithMany(x => x.Nodes)
            .HasForeignKey(x => x.StoryDialogTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogNodeTranslationConfiguration : IEntityTypeConfiguration<StoryDialogNodeTranslation>
{
    public void Configure(EntityTypeBuilder<StoryDialogNodeTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryDialogNodeId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryDialogNode)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.StoryDialogNodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogEdgeConfiguration : IEntityTypeConfiguration<StoryDialogEdge>
{
    public void Configure(EntityTypeBuilder<StoryDialogEdge> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryDialogNodeId, x.EdgeId }).IsUnique();
        builder.HasOne(x => x.StoryDialogNode)
            .WithMany(x => x.OutgoingEdges)
            .HasForeignKey(x => x.StoryDialogNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.JumpToTileId).HasMaxLength(100);
        builder.Property(x => x.SetBranchId).HasMaxLength(100);
    }
}

public class StoryDialogEdgeTranslationConfiguration : IEntityTypeConfiguration<StoryDialogEdgeTranslation>
{
    public void Configure(EntityTypeBuilder<StoryDialogEdgeTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryDialogEdgeId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryDialogEdge)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.StoryDialogEdgeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryCraftDialogEdgeTokenConfiguration : IEntityTypeConfiguration<StoryCraftDialogEdgeToken>
{
    public void Configure(EntityTypeBuilder<StoryCraftDialogEdgeToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.Value).HasMaxLength(200);
        builder.HasIndex(x => x.StoryCraftDialogEdgeId);
        builder.HasOne(x => x.StoryCraftDialogEdge)
            .WithMany(x => x.Tokens)
            .HasForeignKey(x => x.StoryCraftDialogEdgeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StoryDialogEdgeTokenConfiguration : IEntityTypeConfiguration<StoryDialogEdgeToken>
{
    public void Configure(EntityTypeBuilder<StoryDialogEdgeToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.Value).HasMaxLength(200);
        builder.HasIndex(x => x.StoryDialogEdgeId);
        builder.HasOne(x => x.StoryDialogEdge)
            .WithMany(x => x.Tokens)
            .HasForeignKey(x => x.StoryDialogEdgeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
