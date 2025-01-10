using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models.Configurations;
using SmartKeyCaddy.Models.Exceptions;

namespace SmartKeyCaddy.Domain.Services;

public partial class StorageContainerService : IStorageContainerService
{
    private readonly ILogger<StorageContainerService> _logger;
    private readonly AzureStorageContainerSettings _azureStorageContainerSettings;

    public StorageContainerService(ILogger<StorageContainerService> logger,
        IOptions<AzureStorageContainerSettings> azureStorageContainerSettings)
    {
        _logger = logger;
        _azureStorageContainerSettings = azureStorageContainerSettings.Value;
    }

    public async Task<byte[]> GetBlob(string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_azureStorageContainerSettings.ConnectionString);

        // Get the container client
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
            throw new NotFoundException("Blob not exists");

        var blobContent = await blobClient.DownloadContentAsync();

        return blobContent.Value.Content.ToArray();
    }
}