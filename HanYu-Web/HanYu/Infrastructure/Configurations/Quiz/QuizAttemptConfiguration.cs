using HanYu.Domain.Entities.Quiz;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Quiz;

public sealed class QuizAttemptConfiguration
    : TimestampedEntityConfigurationBase<QuizAttempt>
{
    public override void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        base.Configure(builder);

        builder.ToTable("quiz_attempts");

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.IdempotencyKey)
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.QuizId, x.AttemptNumber })
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Score).HasPrecision(8, 2);
        builder.Property(x => x.MaxScore).HasPrecision(8, 2);
        builder.Property(x => x.Percentage).HasPrecision(5, 2);

        builder.Property(x => x.StartedAt).HasColumnType("timestamptz");
        builder.Property(x => x.SubmittedAt).HasColumnType("timestamptz");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.StartedAt });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Quiz)
            .WithMany()
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
