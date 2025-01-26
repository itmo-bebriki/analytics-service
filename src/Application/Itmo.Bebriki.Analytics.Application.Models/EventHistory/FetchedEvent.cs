namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public record FetchedEvent(
    long Id,
    EventType EventType,
    DateTimeOffset Timestamp,
    string Payload);
