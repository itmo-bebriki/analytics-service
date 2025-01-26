namespace Itmo.Bebriki.Analytics.Application.Models.EventHistory;

public enum EventType
{
    None = 0,
    Creation = 1,
    Update = 2,
    NewDependency = 3,
    PruneDependency = 4,
}
