using Microsoft.AspNetCore.Http.HttpResults;

namespace XooCreator.BA.Features.StoryEditor.Models;

    public record AssetCopyResult
    {
        public bool HasError => ErrorResult != null;
        public Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ErrorResult { get; init; }
        public string? AssetFilename { get; init; }
        public string? ErrorMessage { get; init; }

        public static AssetCopyResult Success() => new AssetCopyResult { ErrorResult = null };
        
        public static AssetCopyResult AssetNotFound(string filename, string storyId)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Draft asset not found: {filename}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Draft asset not found: {filename}"
            };
        }

        public static AssetCopyResult CopyFailed(string filename, string storyId, string reason)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Failed to copy asset '{filename}': {reason}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Failed to copy asset '{filename}': {reason}"
            };
        }

        public static AssetCopyResult CopyTimeout(string filename, string storyId)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Timeout while copying asset '{filename}'. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Timeout while copying asset '{filename}'"
            };
        }
    }
