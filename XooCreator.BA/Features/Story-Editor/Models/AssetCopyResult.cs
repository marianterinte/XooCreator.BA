using Microsoft.AspNetCore.Http.HttpResults;

namespace XooCreator.BA.Features.StoryEditor.Models;

    public sealed record AssetCopyFailure
    {
        public required string Filename { get; init; }
        public required string Type { get; init; }
        public string? Language { get; init; }
        public required string Reason { get; init; }
    }

    public record AssetCopyResult
    {
        public bool HasError => ErrorResult != null;
        public Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ErrorResult { get; init; }
        public string? AssetFilename { get; init; }
        public string? ErrorMessage { get; init; }
        public IReadOnlyList<AssetCopyFailure> FailedAssets { get; init; } = Array.Empty<AssetCopyFailure>();

        public static AssetCopyResult Success(IReadOnlyList<AssetCopyFailure>? failedAssets = null)
            => new AssetCopyResult
            {
                ErrorResult = null,
                FailedAssets = failedAssets ?? Array.Empty<AssetCopyFailure>()
            };
        
        public static AssetCopyResult AssetNotFound(string filename, string storyId, IReadOnlyList<AssetCopyFailure>? failedAssets = null)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Draft asset not found: {filename}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Draft asset not found: {filename}",
                FailedAssets = failedAssets ?? Array.Empty<AssetCopyFailure>()
            };
        }

        public static AssetCopyResult CopyFailed(string filename, string storyId, string reason, IReadOnlyList<AssetCopyFailure>? failedAssets = null)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Failed to copy asset '{filename}': {reason}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Failed to copy asset '{filename}': {reason}",
                FailedAssets = failedAssets ?? Array.Empty<AssetCopyFailure>()
            };
        }

        public static AssetCopyResult CopyTimeout(string filename, string storyId, IReadOnlyList<AssetCopyFailure>? failedAssets = null)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Timeout while copying asset '{filename}'. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Timeout while copying asset '{filename}'",
                FailedAssets = failedAssets ?? Array.Empty<AssetCopyFailure>()
            };
        }
    }
