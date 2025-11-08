using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace XooCreator.BA.Infrastructure.Services.Blob;

public interface IBlobSasService
{
    string DraftContainer { get; }
    string PublishedContainer { get; }

    Task<Uri> GetPutSasAsync(string container, string blobPath, string contentType, TimeSpan ttl, CancellationToken ct = default);
    Task<Uri> GetReadSasAsync(string container, string blobPath, TimeSpan ttl, CancellationToken ct = default);
    BlobClient GetBlobClient(string container, string blobPath);
}

