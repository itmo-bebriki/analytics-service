using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IEventHistoryRepository eventHistoryRepository,
        IAnalyticsRepository analyticsRepository)
    {
        EventHistoryRepository = eventHistoryRepository;
        AnalyticsRepository = analyticsRepository;
    }

    public IAnalyticsRepository AnalyticsRepository { get; set; }

    public IEventHistoryRepository EventHistoryRepository { get; set; }
}
