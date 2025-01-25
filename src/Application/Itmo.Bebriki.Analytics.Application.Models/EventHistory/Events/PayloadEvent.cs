namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory.Events;

public record PayloadEvent(
    long Id,
    EventType EventType,
    DateTimeOffset Timestamp,
    string Payload)
    : BaseEvent(Id, EventType, Timestamp);
