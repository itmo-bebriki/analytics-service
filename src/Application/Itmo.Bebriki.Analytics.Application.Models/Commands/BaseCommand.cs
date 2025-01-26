using System.Text.Json.Serialization;

namespace Itmo.Bebriki.Analytics.Application.Models.Commands;

[JsonDerivedType(typeof(BaseCommand), typeDiscriminator: "base")]
[JsonDerivedType(typeof(CreateJobTaskCommand), typeDiscriminator: "create_job_task")]
[JsonDerivedType(typeof(UpdateJobTaskCommand), typeDiscriminator: "update_job_task")]
[JsonDerivedType(typeof(DependencyCommand), typeDiscriminator: "dependency")]
public record BaseCommand(long Id);
