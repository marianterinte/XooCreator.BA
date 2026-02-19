using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

public class StoryCraftDialogParticipant
{
    public Guid Id { get; set; }
    public Guid StoryCraftId { get; set; }

    [MaxLength(100)]
    public string HeroId { get; set; } = string.Empty;

    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoryCraft StoryCraft { get; set; } = null!;
}

public class StoryCraftDialogTile
{
    public Guid Id { get; set; }
    public Guid StoryCraftId { get; set; }
    public Guid StoryCraftTileId { get; set; }
    public string? RootNodeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public StoryCraft StoryCraft { get; set; } = null!;
    public StoryCraftTile StoryCraftTile { get; set; } = null!;
    public List<StoryCraftDialogNode> Nodes { get; set; } = new();
}

public class StoryCraftDialogNode
{
    public Guid Id { get; set; }
    public Guid StoryCraftDialogTileId { get; set; }

    [MaxLength(100)]
    public string NodeId { get; set; } = string.Empty;

    [MaxLength(20)]
    public string SpeakerType { get; set; } = "reader";

    [MaxLength(100)]
    public string? SpeakerHeroId { get; set; }

    public int SortOrder { get; set; }

    public StoryCraftDialogTile StoryCraftDialogTile { get; set; } = null!;
    public List<StoryCraftDialogNodeTranslation> Translations { get; set; } = new();
    public List<StoryCraftDialogEdge> OutgoingEdges { get; set; } = new();
}

public class StoryCraftDialogNodeTranslation
{
    public Guid Id { get; set; }
    public Guid StoryCraftDialogNodeId { get; set; }

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "ro-ro";

    public string Text { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    public StoryCraftDialogNode StoryCraftDialogNode { get; set; } = null!;
}

public class StoryCraftDialogEdge
{
    public Guid Id { get; set; }
    public Guid StoryCraftDialogNodeId { get; set; }

    [MaxLength(100)]
    public string EdgeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ToNodeId { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? JumpToTileId { get; set; }
    [MaxLength(100)]
    public string? SetBranchId { get; set; }

    public int OptionOrder { get; set; }

    [MaxLength(100)]
    public string? HideIfBranchSet { get; set; }

    /// <summary>JSON array of branch IDs, e.g. ["b1","b2"]. Stored as TEXT.</summary>
    public string? ShowOnlyIfBranchesSet { get; set; }

    public StoryCraftDialogNode StoryCraftDialogNode { get; set; } = null!;
    public List<StoryCraftDialogEdgeTranslation> Translations { get; set; } = new();
    public List<StoryCraftDialogEdgeToken> Tokens { get; set; } = new();
}

public class StoryCraftDialogEdgeTranslation
{
    public Guid Id { get; set; }
    public Guid StoryCraftDialogEdgeId { get; set; }

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "ro-ro";

    public string OptionText { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    public StoryCraftDialogEdge StoryCraftDialogEdge { get; set; } = null!;
}

public class StoryCraftDialogEdgeToken
{
    public Guid Id { get; set; }
    public Guid StoryCraftDialogEdgeId { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public StoryCraftDialogEdge StoryCraftDialogEdge { get; set; } = null!;
}

public class StoryDialogParticipant
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }

    [MaxLength(100)]
    public string HeroId { get; set; } = string.Empty;

    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoryDefinition StoryDefinition { get; set; } = null!;
}

public class StoryDialogTile
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public Guid StoryTileId { get; set; }
    public string? RootNodeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public StoryDefinition StoryDefinition { get; set; } = null!;
    public StoryTile StoryTile { get; set; } = null!;
    public List<StoryDialogNode> Nodes { get; set; } = new();
}

public class StoryDialogNode
{
    public Guid Id { get; set; }
    public Guid StoryDialogTileId { get; set; }

    [MaxLength(100)]
    public string NodeId { get; set; } = string.Empty;

    [MaxLength(20)]
    public string SpeakerType { get; set; } = "reader";

    [MaxLength(100)]
    public string? SpeakerHeroId { get; set; }

    public int SortOrder { get; set; }

    public StoryDialogTile StoryDialogTile { get; set; } = null!;
    public List<StoryDialogNodeTranslation> Translations { get; set; } = new();
    public List<StoryDialogEdge> OutgoingEdges { get; set; } = new();
}

public class StoryDialogNodeTranslation
{
    public Guid Id { get; set; }
    public Guid StoryDialogNodeId { get; set; }

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "ro-ro";

    public string Text { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    public StoryDialogNode StoryDialogNode { get; set; } = null!;
}

public class StoryDialogEdge
{
    public Guid Id { get; set; }
    public Guid StoryDialogNodeId { get; set; }

    [MaxLength(100)]
    public string EdgeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ToNodeId { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? JumpToTileId { get; set; }
    [MaxLength(100)]
    public string? SetBranchId { get; set; }

    public int OptionOrder { get; set; }

    [MaxLength(100)]
    public string? HideIfBranchSet { get; set; }

    /// <summary>JSON array of branch IDs, e.g. ["b1","b2"]. Stored as TEXT.</summary>
    public string? ShowOnlyIfBranchesSet { get; set; }

    public StoryDialogNode StoryDialogNode { get; set; } = null!;
    public List<StoryDialogEdgeTranslation> Translations { get; set; } = new();
    public List<StoryDialogEdgeToken> Tokens { get; set; } = new();
}

public class StoryDialogEdgeTranslation
{
    public Guid Id { get; set; }
    public Guid StoryDialogEdgeId { get; set; }

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "ro-ro";

    public string OptionText { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    public StoryDialogEdge StoryDialogEdge { get; set; } = null!;
}

public class StoryDialogEdgeToken
{
    public Guid Id { get; set; }
    public Guid StoryDialogEdgeId { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public StoryDialogEdge StoryDialogEdge { get; set; } = null!;
}
