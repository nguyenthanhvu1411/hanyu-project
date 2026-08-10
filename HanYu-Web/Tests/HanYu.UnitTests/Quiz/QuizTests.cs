using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Enums;

namespace HanYu.UnitTests.Quiz;

public sealed class QuizTests
{
    [Fact]
    public void Constructor_CreatesDraftQuiz()
    {
        var quiz =
            Create();

        quiz.Status.Should().Be(
            ContentStatus.Draft);

        quiz.TitleVi.Should().Be(
            "Quiz Integration");

        quiz.PassingScore.Should().Be(70m);
    }

    [Fact]
    public void Lifecycle_ToPublished_Works()
    {
        var quiz =
            Create();

        // Add a question via reflection because Questions has a private setter or is not easily settable
        var questionsProperty = typeof(Domain.Entities.Quiz.Quiz).GetProperty("Questions");
        var questions = (ICollection<QuizQuestion>)questionsProperty!.GetValue(quiz)!;
        questions.Add(new QuizQuestion(1, QuizQuestionType.MultipleChoice, "Question 1", 1, 0));

        quiz.SubmitForReview();

        quiz.Status.Should().Be(
            ContentStatus.Review);

        quiz.Approve();

        quiz.Status.Should().Be(
            ContentStatus.Approved);

        quiz.Publish();

        quiz.Status.Should().Be(
            ContentStatus.Published);
    }

    [Fact]
    public void InvalidPassingScore_Throws()
    {
        var action =
            () =>
                new Domain.Entities.Quiz.Quiz(
                    "Quiz",
                    QuizType.Lesson,
                    101m);

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AttachInvalidLesson_Throws()
    {
        var quiz =
            Create();

        var action =
            () => quiz.AttachLesson(0);

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    private static Domain.Entities.Quiz.Quiz
        Create()
        => new(
            "Quiz Integration",
            QuizType.Lesson,
            70m);
}
