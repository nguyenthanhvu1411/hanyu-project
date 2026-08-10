using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Microsoft.Extensions.Logging;

namespace HanYu.IntegrationTests.Common;

public sealed class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public new const string Scheme =
        "IntegrationTest";

    public const string UserIdHeader =
        "X-Test-User-Id";

    public const string RoleHeader =
        "X-Test-Role";

    public const string AllowAllHeader =
        "X-Test-Allow-All";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(
            options,
            logger,
            encoder)
    {
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(
                UserIdHeader,
                out var rawUserId))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        if (!Guid.TryParse(
                rawUserId.ToString(),
                out var userId))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Invalid integration test user."));
        }

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new(
                    "sub",
                    userId.ToString()),

                new(
                    ClaimTypes.Name,
                    $"test-{userId:N}")
            };

        if (Request.Headers.TryGetValue(
                RoleHeader,
                out var role) &&
            !string.IsNullOrWhiteSpace(
                role.ToString()))
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role.ToString()));
        }

        if (Request.Headers.TryGetValue(
                AllowAllHeader,
                out var allowAll) &&
            string.Equals(
                allowAll.ToString(),
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(
                new Claim(
                    "integration:allow-all",
                    "true"));
        }

        var identity =
            new ClaimsIdentity(
                claims,
                Scheme);

        var principal =
            new ClaimsPrincipal(identity);

        var ticket =
            new AuthenticationTicket(
                principal,
                Scheme);

        return Task.FromResult(
            AuthenticateResult.Success(
                ticket));
    }
}