using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SaveGeneratedAudioEndpoint
{
    private readonly IBlobSasService _blobService;
    private readonly IAuth0UserService _auth0;

    public SaveGeneratedAudioEndpoint(IBlobSasService blobService, IAuth0UserService auth0)
    {
        _blobService = blobService;
        _auth0 = auth0;
    }

    public record SaveAudioRequest(
        string AudioData, // Base64-encoded audio
        string TileId,
        string StoryId,
        string LanguageCode
    );

    public record SaveAudioResponse(
        string AudioUrl,
        string BlobPath,
        string FileName
    );

    [Route("/api/{locale}/story-editor/save-audio")]
    [Authorize]
    public static async Task<Results<Ok<SaveAudioResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] SaveGeneratedAudioEndpoint ep,
        [FromBody] SaveAudioRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.AudioData))
        {
            return TypedResults.BadRequest("AudioData is required");
        }

        if (string.IsNullOrWhiteSpace(request.TileId))
        {
            return TypedResults.BadRequest("TileId is required");
        }

        if (string.IsNullOrWhiteSpace(request.StoryId))
        {
            return TypedResults.BadRequest("StoryId is required");
        }

        if (string.IsNullOrWhiteSpace(request.LanguageCode))
        {
            return TypedResults.BadRequest("LanguageCode is required");
        }

        try
        {
            // Decode base64 audio data
            byte[] audioBytes;
            try
            {
                audioBytes = Convert.FromBase64String(request.AudioData);
            }
            catch (FormatException)
            {
                return TypedResults.BadRequest("Invalid base64 audio data");
            }

            // Generate filename: {tileId}_audio.wav
            var fileName = $"{request.TileId}_audio.wav";

            // Build blob path using existing mapper
            var assetInfo = new StoryAssetPathMapper.AssetInfo(fileName, StoryAssetPathMapper.AssetType.Audio, request.LanguageCode);
            var blobPath = StoryAssetPathMapper.BuildDraftPath(assetInfo, user.Email, request.StoryId);

            // Upload to Azure Blob Storage
            var blobClient = ep._blobService.GetBlobClient(ep._blobService.DraftContainer, blobPath);
            
            using (var stream = new MemoryStream(audioBytes))
            {
                await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);
            }

            // Set content type
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: ct);
            var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = "audio/wav"
            };
            await blobClient.SetHttpHeadersAsync(headers, cancellationToken: ct);

            var audioUrl = blobClient.Uri.ToString();

            return TypedResults.Ok(new SaveAudioResponse(
                AudioUrl: audioUrl,
                BlobPath: blobPath,
                FileName: fileName
            ));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to save audio: {ex.Message}");
        }
    }
}

