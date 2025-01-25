namespace Itmo.Bebriki.Analytics.Application.Contracts.Commands;

public record DependencyCommand(
    long JobTaskId,
    long[] Dependencies);
