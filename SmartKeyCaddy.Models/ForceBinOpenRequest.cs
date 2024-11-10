using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Models;

public class ForceBinOpenRequest : BaseMessage
{
    public Guid BinId { get; set; }
}