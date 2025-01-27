using Itmo.Bebriki.Analytics.Application.Models.EventHistory;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public sealed record FetchQuery(
    long[] JobTaskIds,
    EventType[] Types,
    DateTimeOffset? FromTimestamp,
    DateTimeOffset? ToTimestamp,
    long Cursor,
    int PageSize);
