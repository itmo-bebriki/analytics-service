namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

public record DependencyCommand(
    long JobTaskId,
    long[] Dependencies)
    : BaseCommand(JobTaskId);
