namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public record RemoveDependencyQuery(long Id, long[] Dependencies);
