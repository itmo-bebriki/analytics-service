using Itmo.Bebriki.Analytics.Application.Models.Commands;

namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public record PayloadEvent(
    long Id,
    EventType EventType,
    DateTimeOffset Timestamp,
    BaseCommand Command);
