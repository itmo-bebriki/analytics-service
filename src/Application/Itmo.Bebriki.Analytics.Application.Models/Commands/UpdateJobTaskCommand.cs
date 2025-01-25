using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

public record UpdateJobTaskCommand(
    long JobTaskId,
    string? Title,
    string? Description,
    long? AssigneeId,
    JobTaskState? State,
    JobTaskPriority? Priority,
    DateTimeOffset? Deadline,
    bool? IsAgreed,
    DateTimeOffset UpdatedAt)
    : BaseCommand(JobTaskId);
