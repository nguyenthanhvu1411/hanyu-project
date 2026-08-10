using HanYu.Domain.Entities.Content;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Content;

using Common;

public sealed class ContentIntegrationTests
    : IntegrationTestBase
{
    public ContentIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ImportJob_CanPersist()
    {
        var job =
            new ContentImportJob(
                default,
                "test.xlsx",
                "/test/test.xlsx");

        job.Start(2);

        job.RegisterProcessedRow(true);

        job.RegisterProcessedRow(false);

        job.Complete();

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(job);

                await db.SaveChangesAsync();
            });

        var stored =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<ContentImportJob>()
                        .SingleAsync(
                            x =>
                                x.Id ==
                                job.Id));

        stored.Status.Should().Be(
            ContentImportStatus.CompletedWithErrors);

        stored.ProcessedRows.Should().Be(2);
    }
}