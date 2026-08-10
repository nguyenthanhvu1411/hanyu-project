using System.Net;

namespace HanYu.IntegrationTests.Identity;

using Common;

public sealed class IdentityIntegrationTests
    : IntegrationTestBase
{
    public IdentityIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        var client =
            Factory.CreateAnonymousClient();

        var suffix =
            Guid.NewGuid()
                .ToString("N");

        var response =
            await client.PostAsJsonAsync(
                "/api/v1/auth/register",
                new
                {
                    userName =
                        $"user_{suffix}",

                    email =
                        $"user_{suffix}@example.com",

                    password =
                        "TestPassword123!",

                    displayName =
                        "Integration User"
                });

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Fails()
    {
        var client =
            Factory.CreateAnonymousClient();

        var suffix =
            Guid.NewGuid()
                .ToString("N");

        var email =
            $"duplicate_{suffix}@example.com";

        var first =
            await client.PostAsJsonAsync(
                "/api/v1/auth/register",
                new
                {
                    userName =
                        $"first_{suffix}",

                    email,

                    password =
                        "TestPassword123!",

                    displayName =
                        "First"
                });

        first.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var second =
            await client.PostAsJsonAsync(
                "/api/v1/auth/register",
                new
                {
                    userName =
                        $"second_{suffix}",

                    email,

                    password =
                        "TestPassword123!",

                    displayName =
                        "Second"
                });

        second.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAccessToken()
    {
        var client =
            Factory.CreateAnonymousClient();

        var suffix =
            Guid.NewGuid()
                .ToString("N");

        var email =
            $"login_{suffix}@example.com";

        const string password =
            "TestPassword123!";

        var register =
            await client.PostAsJsonAsync(
                "/api/v1/auth/register",
                new
                {
                    userName =
                        $"login_{suffix}",

                    email,

                    password,

                    displayName =
                        "Login Integration"
                });

        register.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var login =
            await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email =
                        email,

                    password
                });

        login.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var body =
            await login.Content
                .ReadAsStringAsync();

        body.Should()
            .ContainEquivalentOf(
                "accessToken");
    }

    [Fact]
    public async Task Login_InvalidPassword_Fails()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email =
                        "missing@example.com",

                    password =
                        "Wrong123!"
                });

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized);
    }
}