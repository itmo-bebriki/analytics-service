using Itmo.Bebriki.Analytics.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Itmo.Bebriki.Analytics.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<AnalyticsService>();
        collection.AddScoped<IAnalyticsService, HistoryTrackingServiceWrapper>();
        collection.AddScoped<ITransactionManager, TransactionManager>();

        return collection;
    }
}
