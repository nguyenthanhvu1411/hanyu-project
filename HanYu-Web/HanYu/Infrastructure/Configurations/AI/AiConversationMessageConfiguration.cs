using HanYu.Domain.Entities.AI;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.AI;

public sealed class AiConversationMessageConfiguration
    : EntityConfigurationBase<AiConversationMessage>
{
    public override void Configure(
        EntityTypeBuilder<AiConversationMessage> builder)
    {
        base.Configure(builder);

        builder.ToTable("ai_conversation_messages");

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.MetadataJson)
            .HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(x => new { x.ConversationId, x.CreatedAt });
        builder.HasIndex(x => x.AiRequestId);

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AiRequest>()
            .WithMany()
            .HasForeignKey(x => x.AiRequestId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
