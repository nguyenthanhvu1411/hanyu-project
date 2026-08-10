using HanYu.Domain.Entities.Review;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Review;

public sealed class FlashcardSessionItemConfiguration
    : EntityConfigurationBase<FlashcardSessionItem>
{
    public override void Configure(EntityTypeBuilder<FlashcardSessionItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("flashcard_session_items");

        builder.Property(x => x.Rating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.AnsweredAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.FlashcardSessionId, x.SortOrder })
            .IsUnique();

        builder.HasOne(x => x.Session)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.FlashcardSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
