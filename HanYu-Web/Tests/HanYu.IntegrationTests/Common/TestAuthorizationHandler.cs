using Microsoft.AspNetCore.Authorization;

namespace HanYu.IntegrationTests.Common;

public sealed class TestAuthorizationHandler
    : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IAuthorizationRequirement requirement)
    {
        if (context.User.HasClaim(
                "integration:allow-all",
                "true"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}