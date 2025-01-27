using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Repositories;

public class EventHistoryRepository : IEventHistoryRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;
    private readonly JsonSerializerSettings _settings;

    public EventHistoryRepository(
        IPersistenceConnectionProvider connectionProvider,
        JsonSerializerSettings settings)
    {
        _connectionProvider = connectionProvider;
        _settings = settings;
    }

    public async IAsyncEnumerable<FetchedEvent> QueryAsync(
        FetchQuery ctx,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT
            eh.id,
            eh.job_task_id,
            eh.type,
            eh.occurred_at,
            eh.payload
        FROM event_history AS eh
        WHERE
            :cursor < eh.id
            AND (CARDINALITY(:ids) = 0 OR eh.job_task_id = ANY(:ids))
            AND (CARDINALITY(:types) = 0 OR eh.type = ANY(:types))
            AND (:from_timestamp IS NULL OR eh.occurred_at >= :from_timestamp)
            AND (:to_timestamp IS NULL OR eh.occurred_at <= :to_timestamp)
        LIMIT :page_size;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("cursor", ctx.Cursor)
            .AddParameter("ids", ctx.JobTaskIds)
            .AddParameter("types", ctx.Types)
            .AddParameter("from_timestamp", ctx.FromTimestamp)
            .AddParameter("to_timestamp", ctx.ToTimestamp)
            .AddParameter("page_size", ctx.PageSize);

        await using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new FetchedEvent(
                Id: reader.GetInt64("id"),
                JobTaskId: reader.GetInt64("job_task_id"),
                EventType: reader.GetFieldValue<EventType>("type"),
                Timestamp: reader.GetFieldValue<DateTimeOffset>("occurred_at"),
                Payload: reader.GetString("payload"));
        }
    }

    public async Task AddEventAsync(
        AddEventQuery ctx,
        CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO event_history (
            job_task_id,
            type,
            occurred_at,
            payload
        )
        VALUES (:job_task_id, :evt_type, :occurred_at, :payload::jsonb);
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("job_task_id", ctx.Event.JobTaskId)
            .AddParameter("evt_type", ctx.Event.EventType)
            .AddParameter("occurred_at", ctx.Event.Timestamp)
            .AddParameter("payload", JsonConvert.SerializeObject(
                ctx.Event.Command,
                ctx.Event.GetType(),
                Formatting.Indented,
                _settings));

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
