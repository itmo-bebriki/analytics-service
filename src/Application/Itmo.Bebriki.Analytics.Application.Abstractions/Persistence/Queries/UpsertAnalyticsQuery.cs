using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public record UpsertAnalyticsQuery(
    long Id,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? StartedAt,
    TimeSpan? TimeSpent,
    JobTaskPriority? Priority,
    JobTaskState? State,
    int? AmountOfAgreements,
    int? TotalUpdates);
