using HanYu.Domain.Entities.AI;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.AI;

public sealed class AiRequestConfiguration
    : EntityConfigurationBase<AiRequest>
{
    public override void Configure(EntityTypeBuilder<AiRequest> builder)
    {
        base.Configure(builder);

        builder.ToTable("ai_requests");

        builder.Property(x => x.FeatureType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RequestHash)
            .HasMaxLength(128);

        builder.Property(x => x.PromptVersion)
            .HasMaxLength(30);

        builder.Property(x => x.EstimatedCostUsd)
            .HasPrecision(12, 6);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ErrorCode)
            .HasMaxLength(80);

        builder.Property(x => x.ErrorMessage)
            .HasColumnType("text");

        builder.Property(x => x.RequestedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.CompletedAt)
            .HasColumnType("timestamptz");

        builder.ToTable("ai_requests", t => t.HasCheckConstraint("ck_ai_requests_tokens", "input_tokens >= 0 AND output_tokens >= 0 AND total_tokens >= 0"));

        builder.ToTable("ai_requests", t => t.HasCheckConstraint("ck_ai_requests_latency", "latency_ms IS NULL OR latency_ms >= 0"));

        builder.ToTable("ai_requests", t => t.HasCheckConstraint("ck_ai_requests_cost", "estimated_cost_usd IS NULL OR estimated_cost_usd >= 0"));

        builder.HasIndex(x => new { x.UserId, x.RequestedAt });
        builder.HasIndex(x => new { x.FeatureType, x.RequestHash });
        builder.HasIndex(x => new { x.Status, x.RequestedAt });
        builder.HasIndex(x => x.ConversationId);
        builder.HasIndex(x => x.VocabularyId);
        builder.HasIndex(x => x.LessonId);
        builder.HasIndex(x => x.QuizAttemptAnswerId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<AiConversation>()
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<global::HanYu.Domain.Entities.Vocabulary.Vocabulary>()
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<global::HanYu.Domain.Entities.Lesson.Lesson>()
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<QuizAttemptAnswer>()
            .WithMany()
            .HasForeignKey(x => x.QuizAttemptAnswerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
