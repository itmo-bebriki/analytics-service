using Itmo.Bebriki.Analytics.Contracts;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Mappers;

internal static class EventTypeMapper
{
    internal static EventType FromInternal(Application.Models.EventHistory.EventType eventType)
    {
        return eventType switch
        {
            Application.Models.EventHistory.EventType.Creation => EventType.Creation,
            Application.Models.EventHistory.EventType.Update => EventType.Update,
            Application.Models.EventHistory.EventType.NewDependency => EventType.NewDeps,
            Application.Models.EventHistory.EventType.PruneDependency => EventType.PruneDeps,
            Application.Models.EventHistory.EventType.None => EventType.Unspecified,
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };
    }

    internal static Application.Models.EventHistory.EventType ToInternal(EventType eventType)
    {
        return eventType switch {
            EventType.Unspecified => Application.Models.EventHistory.EventType.None,
            EventType.Creation => Application.Models.EventHistory.EventType.Creation,
            EventType.Update => Application.Models.EventHistory.EventType.Update,
            EventType.NewDeps => Application.Models.EventHistory.EventType.NewDependency,
            EventType.PruneDeps => Application.Models.EventHistory.EventType.PruneDependency,
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };
    }
}
