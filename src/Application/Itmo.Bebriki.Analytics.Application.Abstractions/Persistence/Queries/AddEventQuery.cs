using Itmo.Bebriki.Analytics.Application.Models.EventHistory;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;

public sealed record AddEventQuery(PayloadEvent Event);
