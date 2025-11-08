namespace XooCreator.BA.Features.Assets.DTOs;

public record RequestUploadDto
{
    public required string StoryId { get; init; }
    public required string Lang { get; init; }
    public required string Kind { get; init; } // cover | tile-image | tile-audio | video | meta
    public required string FileName { get; init; }
    public long ExpectedSize { get; init; }
    public string? ContentType { get; init; }
    public string? TileId { get; init; } // optional
}

public record RequestUploadResponse
{
    public required string PutUrl { get; init; }
    public required string BlobUrl { get; init; }
    public required string RelPath { get; init; }
}

public record CommitUploadDto
{
    public required string Container { get; init; }
    public required string BlobPath { get; init; } // full path inside container
    public long? Size { get; init; }
    public string? Sha256 { get; init; }
}

public record CommitUploadResponse
{
    public bool Ok { get; init; }
    public long? Bytes { get; init; }
    public string? ContentType { get; init; }
    public string? ETag { get; init; }
}

