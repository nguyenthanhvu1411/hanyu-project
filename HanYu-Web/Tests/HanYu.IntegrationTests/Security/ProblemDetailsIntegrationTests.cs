using System.Net;

namespace HanYu.IntegrationTests.Security;

using Common;

public sealed class ProblemDetailsIntegrationTests
    : IntegrationTestBase
{
    public ProblemDetailsIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task MissingLesson_Returns404()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/lessons/{Guid.NewGuid()}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MissingLesson_ReturnsJsonError()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/lessons/{Guid.NewGuid()}");

        response.Content.Headers
            .ContentType?
            .MediaType
            .Should()
            .BeOneOf(
                "application/problem+json",
                "application/json");
    }
}