using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Repositories;

public class EventHistoryRepository : IEventHistoryRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public EventHistoryRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<PayloadEvent> QueryAsync(
        FetchQuery ctx,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT
            eh.id,
            eh.type,
            eh.occurred_at,
            eh.payload
        FROM event_history AS eh
        WHERE
            (CARDINALITY(:ids) = 0 OR eh.id = ANY(:ids))
            AND (CARDINALITY(:types) = 0 OR eh.type = ANY(:types))
            AND (:from_timestamp IS NULL OR eh.from_timestamp >= :from_timestamp)
            AND (:to_timestamp IS NULL OR eh.to_timestamp <= :to_timestamp)
        LIMIT :page_size;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("ids", ctx.Ids)
            .AddParameter("types", ctx.Types)
            .AddParameter("from_timestamp", ctx.FromTimestamp)
            .AddParameter("to_timestamp", ctx.ToTimestamp)
            .AddParameter("page_size", ctx.PageSize);

        await using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new PayloadEvent(
                Id: reader.GetInt64("id"),
                EventType: reader.GetFieldValue<EventType>("type"),
                Timestamp: reader.GetDateTime("occurred_at"),
                Command: JsonSerializer.Deserialize<BaseCommand>(reader.GetString("payload")) ?? throw new ArgumentException("payload cannot be empty"));
        }
    }

    public async Task AddEventAsync(
        AddEventQuery ctx,
        CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO event_history
        VALUES (:id, :evt_type, :occurred_at, :payload);
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Event.Id)
            .AddParameter("evt_type", ctx.Event.EventType)
            .AddParameter("occurred_at", ctx.Event.Timestamp)
            .AddParameter("payload", JsonSerializer.Serialize(ctx.Event.Command));

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
