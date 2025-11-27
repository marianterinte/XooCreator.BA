using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace XooCreator.BA.Infrastructure.Services.Blob;

public class BlobSasService : IBlobSasService
{
    private readonly BlobServiceClient _blobServiceClient;
    public string DraftContainer { get; }
    public string PublishedContainer { get; }

    public BlobSasService(IConfiguration configuration)
    {
        var section = configuration.GetSection("AzureStorage");
        var conn = ResolveConfiguredValue(section["ConnectionString"])
                   ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
        if (string.IsNullOrWhiteSpace(conn))
        {
            throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
        }

        DraftContainer = ResolveConfiguredValue(section["DraftContainer"])
                         ?? Environment.GetEnvironmentVariable("AzureStorage__DraftContainer")
                         ?? "alchimalia-drafts";
        PublishedContainer = ResolveConfiguredValue(section["PublishedContainer"])
                             ?? Environment.GetEnvironmentVariable("AzureStorage__PublishedContainer")
                             ?? "alchimaliacontent";

        _blobServiceClient = new BlobServiceClient(conn);
    }

    public BlobClient GetBlobClient(string container, string blobPath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        return containerClient.GetBlobClient(blobPath);
    }

    public BlobContainerClient GetContainerClient(string container)
        => _blobServiceClient.GetBlobContainerClient(container);

    public async Task<Uri> GetPutSasAsync(string container, string blobPath, string contentType, TimeSpan ttl, CancellationToken ct = default)
    {
        // Ensure container exists (no-op if it does)
        await _blobServiceClient.GetBlobContainerClient(container).CreateIfNotExistsAsync(cancellationToken: ct);
        var blobClient = GetBlobClient(container, blobPath);

        var permissions = BlobSasPermissions.Create | BlobSasPermissions.Write;
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sasBuilder.ContentType = contentType;
        sasBuilder.SetPermissions(permissions);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri;
    }

    public Task<Uri> GetReadSasAsync(string container, string blobPath, TimeSpan ttl, CancellationToken ct = default)
    {
        var blobClient = GetBlobClient(container, blobPath);
        var permissions = BlobSasPermissions.Read;
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sasBuilder.SetPermissions(permissions);
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return Task.FromResult(sasUri);
    }
    private static string? ResolveConfiguredValue(string? configured)
    {
        if (string.IsNullOrWhiteSpace(configured))
        {
            return null;
        }

        if (configured.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
        {
            var envKey = configured[4..].Trim();
            return string.IsNullOrWhiteSpace(envKey) ? null : Environment.GetEnvironmentVariable(envKey);
        }

        return configured;
    }
}
