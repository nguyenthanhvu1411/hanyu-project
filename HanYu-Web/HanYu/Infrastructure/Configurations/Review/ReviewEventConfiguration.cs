using HanYu.Domain.Entities.Review;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Review;

public sealed class ReviewEventConfiguration
    : EntityConfigurationBase<ReviewEvent>
{
    public override void Configure(EntityTypeBuilder<ReviewEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("review_events");

        builder.Property(x => x.Rating)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.MasteryBefore).HasPrecision(5, 2);
        builder.Property(x => x.MasteryAfter).HasPrecision(5, 2);
        builder.Property(x => x.ReviewedAt).HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.ReviewedAt });
        builder.HasIndex(x => new { x.UserId, x.VocabularyId, x.ReviewedAt });
    }
}
