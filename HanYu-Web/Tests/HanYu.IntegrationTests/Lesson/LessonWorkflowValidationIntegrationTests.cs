using System.Net;
using System.Text.Json;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Lesson;

using Common;

public sealed class LessonWorkflowValidationIntegrationTests : IntegrationTestBase
{
    public LessonWorkflowValidationIntegrationTests(HanYuWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task PublishRequiresAValidLessonStructure()
    {
        var adminId = await CreateUserAsync("admin-workflow-validation");
        var fixture = await Factory.ExecuteDbAsync(async db =>
        {
            var hskId = await db.Set<HanYu.Domain.Entities.Vocabulary.HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync();
            var lesson = new Domain.Entities.Lesson.Lesson(
                hskId,
                Unique("publish-validation"),
                "Publish validation integration");
            db.Add(lesson);
            await db.SaveChangesAsync();
            lesson.SubmitForReview();
            lesson.Approve();
            await db.SaveChangesAsync();
            return (lesson.Id, lesson.Version);
        });

        var client = Factory.CreateAdminClient(adminId);
        var validation = await client.GetAsync($"/api/v1/admin/lessons/{fixture.Id}/validate");
        validation.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var json = JsonDocument.Parse(await validation.Content.ReadAsStringAsync()))
        {
            json.RootElement.GetProperty("isValid").GetBoolean().Should().BeFalse();
            json.RootElement.GetProperty("issues").EnumerateArray()
                .Select(x => x.GetProperty("code").GetString())
                .Should().Contain("Lesson.SectionRequired");
        }

        var publish = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{fixture.Id}/publish",
            new { version = fixture.Version });
        publish.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await publish.Content.ReadAsStringAsync()).Should().Contain("Lesson.NotPublishable");

        var status = await Factory.ExecuteDbAsync(async db =>
            await db.Set<Domain.Entities.Lesson.Lesson>()
                .Where(x => x.Id == fixture.Id)
                .Select(x => x.Status)
                .SingleAsync());
        status.Should().Be(ContentStatus.Approved);
    }
}
