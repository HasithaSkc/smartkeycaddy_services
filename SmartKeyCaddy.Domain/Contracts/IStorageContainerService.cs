
namespace SmartKeyCaddy.Domain.Contracts;

public interface IStorageContainerService
{
    Task<byte[]> GetBlob(string containerName, string blobName);
}
