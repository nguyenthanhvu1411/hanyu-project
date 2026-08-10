using HanYu.Domain.Entities.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Review;

public sealed class UserVocabularyStateConfiguration
    : IEntityTypeConfiguration<UserVocabularyState>
{
    public void Configure(EntityTypeBuilder<UserVocabularyState> builder)
    {
        builder.ToTable("user_vocabulary_states");

        builder.HasKey(x => new { x.UserId, x.VocabularyId });

        builder.Property(x => x.LearningState)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.MasteryScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.LastCorrectAt).HasColumnType("timestamptz");
        builder.Property(x => x.LastReviewedAt).HasColumnType("timestamptz");
        builder.Property(x => x.NextReviewAt).HasColumnType("timestamptz");
        builder.Property(x => x.FirstLearnedAt).HasColumnType("timestamptz");
        builder.Property(x => x.MasteredAt).HasColumnType("timestamptz");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");

        builder.ToTable("user_vocabulary_states", t => t.HasCheckConstraint("ck_user_vocabulary_states_mastery", "mastery_score >= 0 AND mastery_score <= 100"));

        builder.HasIndex(x => new { x.UserId, x.NextReviewAt });
        builder.HasIndex(x => new { x.UserId, x.LearningState });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vocabulary)
            .WithMany()
            .HasForeignKey(x => x.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
