using Itmo.Bebriki.Analytics.Presentation.Kafka.ConsumerHandlers;
using Itmo.Bebriki.Tasks.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string sectionName = "Presentation:Kafka";
        const string consumerKey = "Presentation:Kafka:Consumers";

        collection.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection(sectionName))
            .AddConsumer(consumer => consumer
                .WithKey<JobTaskInfoKey>()
                .WithValue<JobTaskInfoValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:JobTaskInfo"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleInboxWith<JobTaskInfoConsumerHandler>()));

        return collection;
    }
}
