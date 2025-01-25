using Itmo.Bebriki.Analytics.Application.Models.EventHistory.Events;
using SourceKit.Generators.Builder.Annotations;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record AddEventQuery([RequiredValue] BaseEvent Event);
