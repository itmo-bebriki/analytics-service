using Itmo.Bebriki.Analytics.Application.Models;

namespace Itmo.Bebriki.Analytics.Application.Contracts.Commands;

public record CreateJobTaskCommand(
    long JobTaskId,
    string Title,
    string Description,
    long AssigneeId,
    JobTaskPriority Priority,
    long[] DependsOnIds,
    DateTimeOffset Deadline,
    DateTimeOffset CreatedAt);
