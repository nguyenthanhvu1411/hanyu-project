using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Common;

public sealed record TestLearningData(
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,

    long LessonId,
    Guid LessonPublicId,
    Guid SectionPublicId,

    long QuizId,
    Guid QuizPublicId,

    long QuestionId,
    Guid QuestionPublicId,

    long CorrectOptionId,
    Guid CorrectOptionPublicId);

public static class TestDataSeeder
{
    public static async Task SeedReferenceDataAsync(
        HanYuWebApplicationFactory factory)
    {
        await factory.ExecuteDbAsync(
            async db =>
            {
                for (short level = 1;
                     level <= 6;
                     level++)
                {
                    var exists =
                        await db.Set<HskLevel>()
                            .AnyAsync(
                                x =>
                                    x.Id ==
                                    level);

                    if (exists)
                        continue;

                    db.Add(
                        new HskLevel(
                            $"HSK{level}",
                            $"HSK {level}",
                            level));
                }

                await db.SaveChangesAsync();
            });
    }

    public static async Task<TestLearningData>
        SeedLearningDataAsync(
            HanYuWebApplicationFactory factory)
    {
        return await factory.ExecuteDbAsync(
            async db =>
            {
                var suffix =
                    Guid.NewGuid()
                        .ToString("N")[..8];

                // =====================================
                // VOCABULARY
                // =====================================

                var vocabulary =
                    new Domain.Entities.Vocabulary.Vocabulary(
                        hskLevelId: 1,
                        simplified:
                            $"测{suffix}",
                        pinyin:
                            "cè shì",
                        pinyinNormalized:
                            "ce4 shi4",
                        primaryMeaningVi:
                            "kiểm tra");

                db.Add(vocabulary);

                await db.SaveChangesAsync();

                vocabulary.SubmitForReview();

                vocabulary.Approve();

                vocabulary.Publish();

                await db.SaveChangesAsync();

                // =====================================
                // LESSON
                // =====================================

                var lesson =
                    new Domain.Entities.Lesson.Lesson(
                        1,
                        $"integration-{suffix}",
                        "Bài học Integration");

                db.Add(lesson);

                await db.SaveChangesAsync();

                var section =
                    new LessonSection(
                        lesson.Id,
                        LessonSectionType.Introduction,
                        0,
                        "Giới thiệu");

                section.UpdateContent(
                    "Giới thiệu",
                    "Nội dung integration test");

                db.Add(section);

                db.Add(
                    new LessonVocabulary(
                        lesson.Id,
                        vocabulary.Id,
                        0,
                        true));

                await db.SaveChangesAsync();

                lesson.SubmitForReview();

                lesson.Approve();

                lesson.Publish();

                await db.SaveChangesAsync();

                // =====================================
                // QUIZ
                // =====================================

                var quiz =
                    new Domain.Entities.Quiz.Quiz(
                        "Quiz Integration",
                        QuizType.Lesson,
                        60m);

                quiz.AttachLesson(
                    lesson.Id);

                db.Add(quiz);

                await db.SaveChangesAsync();

                var question =
                    new QuizQuestion(
                        quiz.Id,
                        QuizQuestionType.MultipleChoice,
                        "Từ này có nghĩa là gì?",
                        1m,
                        0);

                question.AttachVocabulary(
                    vocabulary.Id);

                db.Add(question);

                await db.SaveChangesAsync();

                var correct =
                    new QuizQuestionOption(
                        question.Id,
                        "Kiểm tra",
                        true,
                        0);

                var wrong =
                    new QuizQuestionOption(
                        question.Id,
                        "Tạm biệt",
                        false,
                        1);

                db.AddRange(
                    correct,
                    wrong);

                await db.SaveChangesAsync();

                question.SubmitForReview();

                question.Approve();

                question.Publish();

                quiz.Questions.Add(question);

                quiz.SubmitForReview();

                quiz.Approve();

                quiz.Publish();

                await db.SaveChangesAsync();

                return new TestLearningData(
                    vocabulary.Id,
                    vocabulary.PublicId,
                    vocabulary.Simplified,

                    lesson.Id,
                    lesson.PublicId,
                    section.PublicId,

                    quiz.Id,
                    quiz.PublicId,

                    question.Id,
                    question.PublicId,

                    correct.Id,
                    correct.PublicId);
            });
    }
}