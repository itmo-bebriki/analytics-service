using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public record UpdateAnalyticsQuery(
    long Id,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? TimeSpent,
    JobTaskPriority? Priority,
    JobTaskState? State,
    int? AmountOfAgreements,
    int? TotalUpdates,
    int? AmountOfUniqueAssignees);
