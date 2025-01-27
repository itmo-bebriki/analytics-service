using Itmo.Bebriki.Analytics.Application.Models.Commands;

namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public record PayloadEvent(
    long Id,
    long JobTaskId,
    EventType EventType,
    DateTimeOffset Timestamp,
    BaseCommand Command);
