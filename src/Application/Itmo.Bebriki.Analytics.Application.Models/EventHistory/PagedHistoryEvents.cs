namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public record PagedHistoryEvents(long? Cursor, IReadOnlyCollection<PayloadEvent> Events);
