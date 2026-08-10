using HanYu.Domain.Entities.Review;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Review;

public sealed class FlashcardSessionConfiguration
    : TimestampedEntityConfigurationBase<FlashcardSession>
{
    public override void Configure(EntityTypeBuilder<FlashcardSession> builder)
    {
        base.Configure(builder);

        builder.ToTable("flashcard_sessions");

        builder.Property(x => x.Mode)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.SourceType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.StartedAt });
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
