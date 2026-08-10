using HanYu.Domain.Entities.AI;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.AI;

public sealed class AiResponseCacheConfiguration
    : TimestampedEntityConfigurationBase<AiResponseCache>
{
    public override void Configure(
        EntityTypeBuilder<AiResponseCache> builder)
    {
        base.Configure(builder);

        builder.ToTable("ai_response_cache");

        builder.Property(x => x.FeatureType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.CacheKey)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(x => x.CacheKey)
            .IsUnique();

        builder.Property(x => x.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PromptVersion)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ResponseJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.HitCount)
            .HasDefaultValue(0);

        builder.Property(x => x.LastAccessedAt)
            .HasColumnType("timestamptz");

        builder.Property(x => x.ExpiresAt)
            .HasColumnType("timestamptz");

        builder.Ignore(x => x.IsExpired);

        builder.ToTable("ai_response_cache", t => t.HasCheckConstraint("ck_ai_response_cache_hit_count", "hit_count >= 0"));

        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => new { x.FeatureType, x.Model, x.PromptVersion });
    }
}
