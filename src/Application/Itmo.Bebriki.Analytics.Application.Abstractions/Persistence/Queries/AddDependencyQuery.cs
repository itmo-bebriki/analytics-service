namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public record AddDependencyQuery(long Id, long[] Dependencies);
