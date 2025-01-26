using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Newtonsoft.Json;

namespace Itmo.Bebriki.Analytics.Application.Mappers;

public static class PayloadMapper
{
    public static BaseCommand ToCommand(string payload, EventType eventType)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        return eventType switch {
            EventType.Creation => JsonConvert.DeserializeObject<CreateJobTaskCommand>(payload, settings) ?? throw new ArgumentException("invalid payload"),
            EventType.Update => JsonConvert.DeserializeObject<UpdateJobTaskCommand>(payload, settings) ?? throw new ArgumentException("invalid payload"),
            EventType.NewDependency => JsonConvert.DeserializeObject<DependencyCommand>(payload, settings) ?? throw new ArgumentException("invalid payload"),
            EventType.PruneDependency => JsonConvert.DeserializeObject<DependencyCommand>(payload, settings) ?? throw new ArgumentException("invalid payload"),
            EventType.None => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };
    }
}
