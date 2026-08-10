using HanYu.Domain.Entities.Operations;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Operations;

public sealed class AuditLogConfiguration : EntityConfigurationBase<AuditLog>
{
    public override void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("audit_logs");

        builder.Property(x => x.Action).HasMaxLength(80).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(100);
        builder.Property(x => x.EntityPublicId).HasMaxLength(100);

        builder.Property(x => x.OldValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.NewValuesJson).HasColumnType("jsonb");
        builder.Property(x => x.ChangedPropertiesJson).HasColumnType("jsonb");

        builder.Property(x => x.IpAddress)
            .HasConversion(PostgresValueConverters.NullableIpAddress)
            .HasColumnType("inet");

        builder.Property(x => x.UserAgent).HasColumnType("text");
        builder.Property(x => x.CorrelationId).HasMaxLength(100);
        builder.Property(x => x.OccurredAt).HasColumnType("timestamptz");

        builder.HasIndex(x => x.OccurredAt);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
        builder.HasIndex(x => new { x.UserId, x.OccurredAt });

        // Compound: admin filter "all actions of type X in time window"
        builder.HasIndex(x => new { x.Action, x.OccurredAt })
            .HasDatabaseName("ix_audit_logs_action_occurred");

        // Partial index: filter by EntityType most frequently (vocabulary, lesson, quiz)
        builder.HasIndex(x => new { x.EntityType, x.EntityPublicId, x.OccurredAt })
            .HasDatabaseName("ix_audit_logs_entity_type_pubid_occurred");
    }
}
