namespace HanYu.Domain.Constants;

public static class ReviewConstants
{
    public const int AgainIntervalMinutes = 10;

    public const int HardFirstIntervalMinutes = 60;

    public const int GoodFirstIntervalMinutes = 1440;

    public const int EasyFirstIntervalMinutes = 4320;

    public const int InitialLessonIntervalMinutes = 60;

    public const int MaxIntervalMinutes =
        60 * 24 * 180;

    public const decimal AgainMasteryDelta = -12m;

    public const decimal HardMasteryDelta = 5m;

    public const decimal GoodMasteryDelta = 10m;

    public const decimal EasyMasteryDelta = 15m;

    public const int DefaultReviewQueueSize = 20;

    public const int MaxReviewQueueSize = 100;

    public const int DefaultFlashcardSize = 20;

    public const int MaxFlashcardSize = 100;
}
