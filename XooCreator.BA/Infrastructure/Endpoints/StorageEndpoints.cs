using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Infrastructure.Endpoints;

[Endpoint]
public class StorageEndpoints
{
    private readonly IConfiguration _config;
    public StorageEndpoints(IConfiguration config)
    {
        _config = config;
    }

    private record SasUploadRequest(string Container, string BlobName, string ContentType);
    private record SasResponse(string SasUrl);
    private record SasDeleteRequest(string Container, string BlobName);

    [Route("/api/storage/blob/sas-upload")]
    [Authorize]
    [HttpPost]
    public static Results<Ok<SasResponse>, BadRequest<string>> GetUploadSas(
        [FromServices] StorageEndpoints ep,
        [FromBody] SasUploadRequest request)
    {
        var conn = ep._config.GetConnectionString("AzureBlob");
        if (string.IsNullOrWhiteSpace(conn)) return TypedResults.BadRequest("Missing AzureBlob connection string");

        var blobUriBuilder = new BlobUriBuilder(new Uri($"https://{new BlobServiceClient(conn).AccountName}.blob.core.windows.net/{request.Container}/{request.BlobName}"));
        var blobClient = new BlobClient(conn, request.Container, request.BlobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = request.Container,
            BlobName = request.BlobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Read);
        sasBuilder.ContentType = request.ContentType;

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return TypedResults.Ok(new SasResponse(sasUri.ToString()));
    }

    [Route("/api/storage/blob/sas-delete")]
    [Authorize]
    [HttpPost]
    public static Results<Ok<SasResponse>, BadRequest<string>> GetDeleteSas(
        [FromServices] StorageEndpoints ep,
        [FromBody] SasDeleteRequest request)
    {
        var conn = ep._config.GetConnectionString("AzureBlob");
        if (string.IsNullOrWhiteSpace(conn)) return TypedResults.BadRequest("Missing AzureBlob connection string");

        var blobClient = new BlobClient(conn, request.Container, request.BlobName);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = request.Container,
            BlobName = request.BlobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Delete);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return TypedResults.Ok(new SasResponse(sasUri.ToString()));
    }
}


