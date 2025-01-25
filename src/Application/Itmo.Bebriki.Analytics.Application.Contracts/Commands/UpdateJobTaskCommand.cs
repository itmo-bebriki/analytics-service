using Itmo.Bebriki.Analytics.Application.Models;

namespace Itmo.Bebriki.Analytics.Application.Contracts.Commands;

public record UpdateJobTaskCommand(
    long JobTaskId,
    string Title,
    string Description,
    long? AssigneeId,
    JobTaskState State,
    JobTaskPriority Priority,
    DateTimeOffset Deadline,
    bool? IsAgreed,
    DateTimeOffset UpdatedAt);
