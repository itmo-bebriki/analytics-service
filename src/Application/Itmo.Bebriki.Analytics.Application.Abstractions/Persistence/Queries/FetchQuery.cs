using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using SourceKit.Generators.Builder.Annotations;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record FetchQuery(
    long[] Ids,
    EventType[] Types,
    DateTimeOffset? FromTimestamp,
    DateTimeOffset? ToTimestamp);
