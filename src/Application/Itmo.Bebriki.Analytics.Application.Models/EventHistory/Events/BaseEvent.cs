using System.Text.Json.Serialization;

namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory.Events;

[JsonDerivedType(typeof(PayloadEvent), typeDiscriminator: nameof(PayloadEvent))]
public record BaseEvent(
    long Id,
    EventType EventType,
    DateTimeOffset Timestamp);
