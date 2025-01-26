using Itmo.Bebriki.Analytics.Presentation.Grpc.Controllers;
using Microsoft.AspNetCore.Builder;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<AnalyticsController>();
            routeBuilder.MapGrpcService<EventHistoryController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}
