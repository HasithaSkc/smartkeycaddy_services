using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;
public partial class KeyFobTagService : IKeyFobTagService
{
    private readonly ILogger<KeyFobTagService> _logger;
    private readonly IKeyFobTagRepository _keyFobTagRepository;

    public KeyFobTagService(ILogger<KeyFobTagService> logger,
        IKeyFobTagRepository keyFobTagRepository)
    {
        _logger = logger;
        _keyFobTagRepository = keyFobTagRepository;
    }

    public Task<List<KeyFobTag>> GetKeyFobTags(Guid propertyId)
    {
        return _keyFobTagRepository.GetKeyFobTags(propertyId);
    }

    public Task<List<KeyFobTag>> GetPropertyRoomKeyFobTags(Guid propertyId)
    {
        return _keyFobTagRepository.GetPropertyRoomKeyFobTags(propertyId);
    }
}
