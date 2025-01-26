using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IAnalyticsRepository AnalyticsRepository { get; }

    IEventHistoryRepository EventHistoryRepository { get; }
}
