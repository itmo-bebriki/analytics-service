using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Application.Models.JobTask;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public AnalyticsRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<TaskAnalytics> QueryAsync(FetchAnalyticsQuery ctx, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT
            ta.id,
            ta.created_at,
            ta.last_update,
            ta.started_at,
            ta.time_spent,
            ta.highest_priority,
            ta.current_state,
            ta.amount_of_agreements,
            ta.total_updates,
            array_length(ta.assignees, 1) AS amount_of_unique_assignees,
            array_length(ta.dependencies, 1) AS amount_of_unique_dependencies
        FROM task_analytics AS ta
        WHERE ta.id = :id;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id);

        await using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

        return new TaskAnalytics(
            Id: reader.GetInt64("id"),
            CreatedAt: reader.GetFieldValue<DateTimeOffset>("created_at"),
            LastUpdate: reader.GetFieldValue<DateTimeOffset>("last_update"),
            StartedAt: reader.GetFieldValue<DateTimeOffset>("started_at"),
            TimeSpent: reader.GetFieldValue<TimeSpan>("time_spent"),
            HighestPriority: reader.GetFieldValue<JobTaskPriority>("highest_priority"),
            CurrentState: reader.GetFieldValue<JobTaskState>("current_state"),
            AmountOfAgreements: reader.GetInt32("amount_of_agreements"),
            TotalUpdates: reader.GetInt32("total_updates"),
            AmountOfUniqueAssignees: reader.GetInt32("amount_of_unique_assignees"),
            AmountOfUniqueDependencies: reader.GetInt32("amount_of_unique_dependencies"));
    }

    public async Task UpsertAsync(UpsertAnalyticsQuery ctx, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO task_analytics
        VALUES (
            COALESCE(:created_at, task_analytics.created_at),
            COALESCE(:last_update, task_analytics.last_update),
            COALESCE(:started_at, task_analytics.started_at),
            COALESCE(:time_spent, task_analytics.time_spent),
            COALESCE(:highest_priority, task_analytics.highest_priority),
            COALESCE(:current_state, task_analytics.current_state),
            COALESCE(:amount_of_agreements, task_analytics.amount_of_agreements),
            COALESCE(:total_updates, task_analytics.total_updates)
        )
        ON CONFLICT (id) DO UPDATE
        SET created_at = EXCLUDED.created_at,
            last_update = EXCLUDED.last_update,
            started_at = EXCLUDED.started_at,
            time_spent = EXCLUDED.time_spent,
            highest_priority = EXCLUDED.highest_priority,
            current_state = EXCLUDED.current_state,
            amount_of_agreements = EXCLUDED.amount_of_agreements,
            total_updates = EXCLUDED.total_updates;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id)
            .AddParameter("created_at", ctx.CreatedAt)
            .AddParameter("last_update", ctx.UpdatedAt)
            .AddParameter("started_at", ctx.StartedAt)
            .AddParameter("time_spent", ctx.TimeSpent)
            .AddParameter("highest_priority", ctx.Priority)
            .AddParameter("current_state", ctx.State)
            .AddParameter("amount_of_agreements", ctx.AmountOfAgreements)
            .AddParameter("total_updates", ctx.TotalUpdates);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddDependencyAsync(AddDependencyQuery ctx, CancellationToken cancellationToken)
    {
        // Remove all entries of dependency ids from the current array
        // to ensure that there are no duplicates after the insertion.
        await RemoveDependencyAsync(new RemoveDependencyQuery(ctx.Id, ctx.Dependencies), cancellationToken);

        const string sql = """
        UPDATE task_analytics
        SET task_analytics.dependencies = task_analytics.dependencies || :dependency_ids
        WHERE task_analytics.id = :id;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id)
            .AddParameter("dependency_ids", ctx.Dependencies);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveDependencyAsync(RemoveDependencyQuery ctx, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE task_analytics
        SET task_analytics.dependencies = array_agg(element)
        FROM unnest(task_analytics.dependencies) AS element
        WHERE task_analytics.id = :id
            AND element <> ALL(:dependency_ids);
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id)
            .AddParameter("dependency_ids", ctx.Dependencies);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddAssigneeAsync(AddAssigneeQuery ctx, CancellationToken cancellationToken)
    {
        // Remove assignee entry if it exists so there will be no duplicates.
        await RemoveAssigneeAsync(new RemoveAssigneeQuery(ctx.Id, ctx.AssigneeId), cancellationToken);

        const string sql = """
        UPDATE task_analytics
        SET task_analytics.assignees = array_append(task_analytics.assignees, :assignee_id)
        WHERE task_analytics.id = :id;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id)
            .AddParameter("assignee_id", ctx.AssigneeId);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAssigneeAsync(RemoveAssigneeQuery ctx, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE task_analytics
        SET task_analytics.assignees = array_remove(task_analytics.assignees, :assignee_id)
        WHERE task_analytics.id = :id;
        """;

        await using IPersistenceConnection conn = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand cmd = conn.CreateCommand(sql)
            .AddParameter("id", ctx.Id)
            .AddParameter("assignee_id", ctx.AssigneeId);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}