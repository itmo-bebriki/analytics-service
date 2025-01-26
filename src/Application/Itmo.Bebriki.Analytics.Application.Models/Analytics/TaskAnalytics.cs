using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application.Models.Analytics;

public record TaskAnalytics(
    long Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdate,
    DateTimeOffset StartedAt,
    TimeSpan TimeSpent,
    JobTaskPriority HighestPriority,
    JobTaskState CurrentState,
    int AmountOfAgreements,
    int TotalUpdates,
    int AmountOfUniqueAssignees,
    int AmountOfUniqueDependencies);
