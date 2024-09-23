using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
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

    public async Task<List<KeyFobTag>> GetKeyFobTags(Guid propertyId)
    {
        return await _keyFobTagRepository.GetKeyFobTags(propertyId);
    }
}
