namespace XooCreator.BA.Features.StoryEditor.Models;

public record StagedMediaFile(
    string ClientFileId,
    string FileName,
    string BlobPath,
    long? ExpectedSize,
    string? ContentType);

public record ImageImportOverride(
    string ClientFileId,
    string TileId);

public record AudioImportOverride(
    string ClientFileId,
    int TargetIndex);

public record ImageBatchMappingPayload(
    List<StagedMediaFile> Files,
    List<ImageImportOverride>? Overrides);

public record AudioBatchMappingPayload(
    List<StagedMediaFile> Files,
    List<AudioImportOverride>? Overrides);
