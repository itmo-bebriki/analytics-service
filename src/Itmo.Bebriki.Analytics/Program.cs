#pragma warning disable CA1506

using Itmo.Bebriki.Analytics.Application.Extensions;
using Itmo.Bebriki.Analytics.Infrastructure.Persistence.Extensions;
using Itmo.Bebriki.Analytics.Presentation.Grpc.Extensions;
using Itmo.Bebriki.Analytics.Presentation.Kafka.Extensions;
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.MessagePersistence.Extensions;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Itmo.Dev.Platform.Observability;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JsonSerializerSettings>();
builder.Services.AddSingleton(sp =>
{
    JsonSerializerSettings settings = sp.GetRequiredService<IOptions<JsonSerializerSettings>>().Value;
    settings.TypeNameHandling = TypeNameHandling.Auto;

    return settings;
});

builder.Services.AddPlatform();
builder.AddPlatformObservability();

builder.Services.AddApplication();
builder.Services.AddInfrastructurePersistence();
builder.Services.AddPresentationGrpc();
builder.Services.AddPresentationKafka(builder.Configuration);

builder.Services.AddPlatformEvents(b => b.AddPresentationKafkaHandlers());
builder.Services.AddUtcDateTimeProvider();

builder.Services.AddOptions<JsonSerializerSettings>()
    .Configure(options => options.NullValueHandling = NullValueHandling.Ignore);

builder.Services.AddPlatformMessagePersistence(selector => selector
    .UsePostgresPersistence(postgres => postgres
        .ConfigureOptions(optionsBuilder => optionsBuilder
            .BindConfiguration("Infrastructure:MessagePersistence:Persistence"))));

WebApplication app = builder.Build();

app.UseRouting();
app.UsePlatformObservability();
app.UsePresentationGrpc();

await app.RunAsync();
