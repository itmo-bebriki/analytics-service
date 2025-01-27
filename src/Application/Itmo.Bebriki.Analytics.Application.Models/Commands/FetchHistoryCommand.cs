using Itmo.Bebriki.Analytics.Application.Models.EventHistory;

namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

public record FetchHistoryCommand(
    long[] JobTaskIds,
    EventType[] Types,
    DateTimeOffset? FromTimestamp,
    DateTimeOffset? ToTimestamp,
    long Cursor,
    int PageSize);
