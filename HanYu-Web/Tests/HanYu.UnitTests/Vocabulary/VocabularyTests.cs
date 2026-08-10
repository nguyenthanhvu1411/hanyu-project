using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.UnitTests.Vocabulary;

public sealed class VocabularyTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesDraftVocabulary()
    {
        var vocabulary =
            CreateVocabulary();

        vocabulary.HskLevelId.Should().Be(1);

        vocabulary.Simplified.Should().Be("你好");

        vocabulary.Pinyin.Should().Be("nǐ hǎo");

        vocabulary.PinyinNormalized.Should().Be("ni3 hao3");

        vocabulary.PrimaryMeaningVi.Should().Be("xin chào");

        vocabulary.Status.Should().Be(
            ContentStatus.Draft);
    }

    [Fact]
    public void SubmitForReview_FromDraft_ChangesStatus()
    {
        var vocabulary =
            CreateVocabulary();

        vocabulary.SubmitForReview();

        vocabulary.Status.Should().Be(
            ContentStatus.Review);
    }

    [Fact]
    public void Approve_FromReview_ChangesStatus()
    {
        var vocabulary =
            CreateVocabulary();

        vocabulary.SubmitForReview();

        vocabulary.Approve();

        vocabulary.Status.Should().Be(
            ContentStatus.Approved);
    }

    [Fact]
    public void Publish_FromApproved_ChangesStatus()
    {
        var vocabulary =
            CreateVocabulary();

        vocabulary.SubmitForReview();

        vocabulary.Approve();

        vocabulary.Publish();

        vocabulary.Status.Should().Be(
            ContentStatus.Published);

        vocabulary.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_FromDraft_Throws()
    {
        var vocabulary =
            CreateVocabulary();

        var action =
            () => vocabulary.Approve();

        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_InvalidHsk_Throws()
    {
        var action =
            () =>
                new Domain.Entities.Vocabulary.Vocabulary(
                    0,
                    "你好",
                    "nǐ hǎo",
                    "ni3 hao3",
                    "xin chào");

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    private static Domain.Entities.Vocabulary.Vocabulary
        CreateVocabulary()
        => new(
            hskLevelId: 1,
            simplified: "你好",
            pinyin: "nǐ hǎo",
            pinyinNormalized: "ni3 hao3",
            primaryMeaningVi: "xin chào");
}
