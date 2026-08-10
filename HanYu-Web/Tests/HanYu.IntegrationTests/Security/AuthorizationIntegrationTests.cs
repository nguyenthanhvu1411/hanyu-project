using System.Net;

namespace HanYu.IntegrationTests.Security;

using Common;

public sealed class AuthorizationIntegrationTests
    : IntegrationTestBase
{
    public AuthorizationIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ReviewQueue_Anonymous_Returns401()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                "/api/v1/public/review-queue");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_Anonymous_Returns401()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                "/api/v1/admin/review-states");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_NormalUser_Returns403()
    {
        var userId =
            await CreateUserAsync();

        var client =
            Factory.CreateUserClient(
                userId);

        var response =
            await client.GetAsync(
                "/api/v1/admin/review-states");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminClient_CanReachAdminEndpoint()
    {
        var adminId =
            await CreateUserAsync(
                "admin");

        var client =
            Factory.CreateAdminClient(
                adminId);

        var response =
            await client.GetAsync(
                "/api/v1/admin/review-states");

        response.StatusCode
            .Should()
            .NotBe(HttpStatusCode.Unauthorized);

        response.StatusCode
            .Should()
            .NotBe(HttpStatusCode.Forbidden);
    }
}