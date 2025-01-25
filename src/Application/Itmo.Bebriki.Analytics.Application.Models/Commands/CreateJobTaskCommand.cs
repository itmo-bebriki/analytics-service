using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

public record CreateJobTaskCommand(
    long JobTaskId,
    string Title,
    string Description,
    long AssigneeId,
    JobTaskPriority Priority,
    long[] DependsOnIds,
    DateTimeOffset Deadline,
    DateTimeOffset CreatedAt)
    : BaseCommand(JobTaskId);
