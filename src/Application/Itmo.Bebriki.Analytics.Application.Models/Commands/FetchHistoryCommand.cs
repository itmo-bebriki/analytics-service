using Itmo.Bebriki.Analytics.Application.Models.EventHistory;

namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

public record FetchHistoryCommand(
    long[] Ids,
    EventType[] Types,
    DateTimeOffset? FromTimestamp,
    DateTimeOffset? ToTimestamp,
    int PageSize);
