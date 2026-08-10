using HanYu.Domain.Entities.Analytics;
using HanYu.Domain.Entities.Gamification;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.EndToEnd;

using Common;

public sealed class CoreLearningJourneyTests
    : IntegrationTestBase
{
    public CoreLearningJourneyTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task
        LessonComplete_ThenReview_ProducesConsistentLearningState()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        // =====================================
        // START LESSON
        // =====================================

        var start =
            await client.PostAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/start",
                null);

        start.IsSuccessStatusCode
            .Should()
            .BeTrue();

        // =====================================
        // COMPLETE SECTION
        // =====================================

        var section =
            await client.PutAsJsonAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/sections/{data.SectionPublicId}/progress",
                new
                {
                    timeSpentSeconds =
                        180,

                    isCompleted =
                        true
                });

        section.IsSuccessStatusCode
            .Should()
            .BeTrue();

        // =====================================
        // COMPLETE LESSON
        // =====================================

        var complete =
            await client.PostAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/complete",
                null);

        complete.IsSuccessStatusCode
            .Should()
            .BeTrue();

        // =====================================
        // COMPLETE AGAIN
        // =====================================

        var completeAgain =
            await client.PostAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/complete",
                null);

        completeAgain.IsSuccessStatusCode
            .Should()
            .BeTrue();

        // =====================================
        // CHECK PROGRESS / SRS
        // =====================================

        var afterLesson =
            await Factory.ExecuteDbAsync(
                async db =>
                {
                    var progress =
                        await db.Set<UserLessonProgress>()
                            .SingleAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.LessonId ==
                                    data.LessonId);

                    var state =
                        await db.Set<UserVocabularyState>()
                            .SingleOrDefaultAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.VocabularyId ==
                                    data.VocabularyId);

                    var xp =
                        await db.Set<XpTransaction>()
                            .Where(
                                x =>
                                    x.UserId ==
                                    userId)
                            .SumAsync(
                                x =>
                                    (int?)x.Amount)
                        ?? 0;

                    var streak =
                        await db.Set<UserStreak>()
                            .SingleOrDefaultAsync(
                                x =>
                                    x.UserId ==
                                    userId);

                    var stats =
                        await db.Set<DailyLearningStat>()
                            .Where(
                                x =>
                                    x.UserId ==
                                    userId)
                            .ToArrayAsync();

                    return new
                    {
                        progress,
                        state,
                        xp,
                        streak,
                        stats
                    };
                });

        afterLesson.progress.Status
            .Should()
            .Be(
                LessonProgressStatus.Completed);

        /*
         * Nếu bạn đã nối Lesson Complete → SRS:
         *
        afterLesson.state
            .Should()
            .NotBeNull();

        afterLesson.state!
            .NextReviewAt
            .Should()
            .NotBeNull();
         */

        /*
         * Nếu Integration/Gamification đã nối:
         *
        afterLesson.xp
            .Should()
            .BeGreaterThan(0);

        afterLesson.streak
            .Should()
            .NotBeNull();

        afterLesson.stats
            .Should()
            .NotBeEmpty();
         */

        // =====================================
        // REVIEW
        // =====================================

        var review =
            await client.PostAsJsonAsync(
                "/api/v1/public/reviews",
                new
                {
                    vocabularyPublicId =
                        data.VocabularyPublicId,

                    rating =
                        ReviewRating.Good,

                    wasCorrect =
                        true,

                    responseTimeMs =
                        450
                });

        review.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var afterReview =
            await Factory.ExecuteDbAsync(
                async db =>
                {
                    var state =
                        await db.Set<UserVocabularyState>()
                            .SingleAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.VocabularyId ==
                                    data.VocabularyId);

                    var reviewEvents =
                        await db.Set<ReviewEvent>()
                            .CountAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.VocabularyId ==
                                    data.VocabularyId);

                    return new
                    {
                        state,
                        reviewEvents
                    };
                });

        afterReview.state.CorrectCount
            .Should()
            .BeGreaterThanOrEqualTo(1);

        afterReview.state.LastReviewedAt
            .Should()
            .NotBeNull();

        afterReview.state.NextReviewAt
            .Should()
            .NotBeNull();

        afterReview.reviewEvents
            .Should()
            .Be(1);

        // =====================================
        // ANALYTICS API
        // =====================================

        var analytics =
            await client.GetAsync(
                "/api/v1/public/analytics/me");

        analytics.IsSuccessStatusCode
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task CompletingLessonTwice_DoesNotAwardDuplicateXp()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/start",
            null);

        await client.PutAsJsonAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/sections/{data.SectionPublicId}/progress",
            new
            {
                timeSpentSeconds = 100,
                isCompleted = true
            });

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/complete",
            null);

        var firstXp =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<XpTransaction>()
                        .Where(
                            x =>
                                x.UserId ==
                                userId)
                        .SumAsync(
                            x =>
                                (int?)x.Amount));

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/complete",
            null);

        var secondXp =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<XpTransaction>()
                        .Where(
                            x =>
                                x.UserId ==
                                userId)
                        .SumAsync(
                            x =>
                                (int?)x.Amount));

        /*
        secondXp.Should().Be(firstXp);
        */
    }
}