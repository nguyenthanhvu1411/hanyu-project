using HanYu.Domain.Entities.Content;
using HanYu.Domain.Enums;

namespace HanYu.UnitTests.Content;

public sealed class ContentImportJobTests
{
    [Fact]
    public void NewJob_IsPending()
    {
        var job =
            CreateJob();

        job.Status.Should().Be(
            ContentImportStatus.Pending);
    }

    [Fact]
    public void Start_ChangesStatusToProcessing()
    {
        var job =
            CreateJob();

        job.Start(2);

        job.Status.Should().Be(
            ContentImportStatus.Processing);

        job.TotalRows.Should().Be(2);

        job.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_AllSuccess_SetsCompleted()
    {
        var job =
            CreateJob();

        job.Start(2);

        job.RegisterProcessedRow(true);

        job.RegisterProcessedRow(true);

        job.Complete();

        job.Status.Should().Be(
            ContentImportStatus.Completed);

        job.SuccessRows.Should().Be(2);

        job.FailedRows.Should().Be(0);
    }

    [Fact]
    public void Complete_WithErrors_SetsCompletedWithErrors()
    {
        var job =
            CreateJob();

        job.Start(2);

        job.RegisterProcessedRow(true);

        job.RegisterProcessedRow(false);

        job.Complete();

        job.Status.Should().Be(
            ContentImportStatus.CompletedWithErrors);

        job.SuccessRows.Should().Be(1);

        job.FailedRows.Should().Be(1);
    }

    [Fact]
    public void CannotCompleteBeforeAllRowsProcessed()
    {
        var job =
            CreateJob();

        job.Start(2);

        job.RegisterProcessedRow(true);

        var action =
            () => job.Complete();

        action.Should()
            .Throw<InvalidOperationException>();
    }

    private static ContentImportJob CreateJob()
        => new(
            default,
            "integration.xlsx",
            "/tests/integration.xlsx");
}
