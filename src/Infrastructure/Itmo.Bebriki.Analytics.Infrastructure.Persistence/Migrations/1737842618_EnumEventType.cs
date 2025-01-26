using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Migrations;

#pragma warning disable SA1649

[Migration(1737842618, "enum event type")]
public class EnumEventType : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return
        """
        CREATE TYPE event_type AS ENUM (
            'creation',
            'update',
            'new_dependency',
            'prune_dependency'
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return
        """
        DROP TYPE event_type;
        """;
    }
}
