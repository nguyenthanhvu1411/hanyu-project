using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Identity;

public class UserPreference : TimestampedEntity
{
    private static readonly string[] AllowedThemes =
    [
        "System",
        "Light",
        "Dark"
    ];

    private static readonly string[] AllowedFlashcardModes =
    [
        "HanziToMeaning",
        "MeaningToHanzi",
        "PinyinToHanzi"
    ];

    public Guid UserId { get; private set; }

    public bool ShowPinyin { get; private set; }
        = true;

    public bool ShowTraditional { get; private set; }

    public bool AutoPlayAudio { get; private set; }

    public decimal AudioPlaybackRate { get; private set; }
        = 1.00m;

    public string Theme { get; private set; }
        = "System";

    public string DefaultFlashcardMode { get; private set; }
        = "HanziToMeaning";

    public bool ReducedMotion { get; private set; }

    public User User { get; private set; } = null!;

    protected UserPreference()
    {
    }

    public UserPreference(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;
    }

    public void UpdateDisplayPreferences(
        bool showPinyin,
        bool showTraditional,
        bool reducedMotion)
    {
        ShowPinyin = showPinyin;
        ShowTraditional = showTraditional;
        ReducedMotion = reducedMotion;

        MarkUpdated();
    }

    public void UpdateAudioPreferences(
        bool autoPlayAudio,
        decimal playbackRate)
    {
        if (playbackRate < 0.5m ||
            playbackRate > 2.0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(playbackRate),
                "Playback rate phải từ 0.5 đến 2.0.");
        }

        AutoPlayAudio = autoPlayAudio;
        AudioPlaybackRate = playbackRate;

        MarkUpdated();
    }

    public void UpdateTheme(string theme)
    {
        if (string.IsNullOrWhiteSpace(theme))
            throw new ArgumentException(
                "Theme không được để trống.",
                nameof(theme));

        theme = theme.Trim();

        if (!AllowedThemes.Contains(
                theme,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Theme không hợp lệ.",
                nameof(theme));
        }

        Theme = AllowedThemes.First(
            x => string.Equals(
                x,
                theme,
                StringComparison.OrdinalIgnoreCase));

        MarkUpdated();
    }

    public void UpdateFlashcardMode(
        string mode)
    {
        if (string.IsNullOrWhiteSpace(mode))
            throw new ArgumentException(
                "Flashcard mode không được để trống.",
                nameof(mode));

        mode = mode.Trim();

        if (!AllowedFlashcardModes.Contains(
                mode,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Flashcard mode không hợp lệ.",
                nameof(mode));
        }

        DefaultFlashcardMode =
            AllowedFlashcardModes.First(
                x => string.Equals(
                    x,
                    mode,
                    StringComparison.OrdinalIgnoreCase));

        MarkUpdated();
    }

    public void ResetToDefault()
    {
        ShowPinyin = true;
        ShowTraditional = false;
        AutoPlayAudio = false;
        AudioPlaybackRate = 1.00m;
        Theme = "System";
        DefaultFlashcardMode =
            "HanziToMeaning";
        ReducedMotion = false;

        MarkUpdated();
    }
}
