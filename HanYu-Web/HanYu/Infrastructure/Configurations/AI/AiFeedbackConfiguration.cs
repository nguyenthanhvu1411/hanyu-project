using HanYu.Domain.Entities.AI;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.AI;

public sealed class AiFeedbackConfiguration
    : EntityConfigurationBase<AiFeedback>
{
    public override void Configure(EntityTypeBuilder<AiFeedback> builder)
    {
        base.Configure(builder);

        builder.ToTable("ai_feedback");

        builder.Property(x => x.Rating)
            .HasConversion<short>();

        builder.Property(x => x.Comment)
            .HasColumnType("text");

        builder.Property(x => x.IssueType)
            .HasMaxLength(80);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(x => new { x.AiRequestId, x.UserId })
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AiRequest>()
            .WithMany()
            .HasForeignKey(x => x.AiRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
