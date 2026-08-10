using HanYu.Domain.Entities.AI;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.AI;

public sealed class AiConversationConfiguration
    : TimestampedEntityConfigurationBase<AiConversation>
{
    public override void Configure(EntityTypeBuilder<AiConversation> builder)
    {
        base.Configure(builder);

        builder.ToTable("ai_conversations");

        builder.Property(x => x.Title)
            .HasMaxLength(220);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.MessageCount)
            .HasDefaultValue(0);

        builder.Property(x => x.LastMessageAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.LastMessageAt });
        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.ToTable("ai_conversations", t => t.HasCheckConstraint("ck_ai_conversations_message_count", "message_count >= 0"));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<global::HanYu.Domain.Entities.Lesson.Lesson>()
            .WithMany()
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<global::HanYu.Domain.Entities.Vocabulary.Vocabulary>()
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
