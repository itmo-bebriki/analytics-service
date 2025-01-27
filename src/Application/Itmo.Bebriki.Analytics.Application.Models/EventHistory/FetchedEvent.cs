namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public record FetchedEvent(
    long Id,
    long JobTaskId,
    EventType EventType,
    DateTimeOffset Timestamp,
    string Payload);
