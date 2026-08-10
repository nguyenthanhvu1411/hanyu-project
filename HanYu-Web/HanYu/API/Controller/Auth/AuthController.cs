using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Features.Identity.Login;
using HanYu.Application.Features.Identity.Logout;
using HanYu.Application.Features.Identity.Me;
using HanYu.Application.Features.Identity.RefreshToken;
using HanYu.Application.Features.Identity.Register;
using HanYu.Application.Features.Identity.VerifyEmail;
using HanYu.Application.Features.Identity.ForgotPassword;
using HanYu.Application.Features.Identity.ResetPassword;
using HanYu.Application.Features.Identity.ChangePassword;
using HanYu.Application.Features.Identity.ResendVerificationEmail;
using HanYu.Application.Features.Identity.RevokeSession;
using HanYu.Application.Features.Identity.SecurityEvents;
using HanYu.Application.Features.Identity.Sessions;
using HanYu.Application.Features.Identity.TwoFactor.Disable;
using HanYu.Application.Features.Identity.TwoFactor.Enable;
using HanYu.Application.Features.Identity.TwoFactor.Login;
using HanYu.Application.Features.Identity.TwoFactor.RecoveryCodes;
using HanYu.Application.Features.Identity.TwoFactor.RegenerateKey;
using HanYu.Application.Features.Identity.TwoFactor.Setup;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace HanYu.API.Controller.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.Register)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.Auth)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginService service,
        CancellationToken cancellationToken)
    {
        var client =
            new LoginClientInfo(
                GetIpAddress(),
                Request.Headers.UserAgent.ToString());

        var result =
            await service.ExecuteAsync(
                request,
                client,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        [FromServices] RefreshTokenService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                GetIpAddress(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        [FromServices] LogoutService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                GetIpAddress(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(
        [FromServices] GetCurrentUserService service,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        [FromServices] VerifyEmailService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] ForgotPasswordService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            request,
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        [FromServices] ResetPasswordService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromServices] ChangePasswordService service,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("resend-verification-email")]
    [AllowAnonymous]
    public async Task<IActionResult>
        ResendVerificationEmail(
            [FromBody]
            ResendVerificationEmailRequest request,

            [FromServices]
            ResendVerificationEmailService service,

            CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            request,
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions(
        [FromServices] GetSessionsService service,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                currentUser.SessionKey,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("sessions/current")]
    [Authorize]
    public async Task<IActionResult> GetCurrentSession(
        [FromServices]
        GetCurrentSessionService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue ||
            !currentUser.SessionKey.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                currentUser.SessionKey.Value,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("sessions/{sessionKey:guid}/revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeSession(
        Guid sessionKey,

        [FromServices]
        RevokeSessionService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                sessionKey,
                currentUser.SessionKey,
                GetIpAddress(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("sessions/revoke-others")]
    [Authorize]
    public async Task<IActionResult>
        RevokeAllOtherSessions(
            [FromServices]
            RevokeAllOtherSessionsService service,

            [FromServices]
            ICurrentUserService currentUser,

            CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue ||
            !currentUser.SessionKey.HasValue)
        {
            return Unauthorized();
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                currentUser.SessionKey.Value,
                GetIpAddress(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("security-events")]
    [Authorize]
    public async Task<IActionResult> GetSecurityEvents(
        [FromQuery] int take,

        [FromServices]
        GetSecurityEventsService service,

        [FromServices]
        ICurrentUserService currentUser,

        CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        if (take <= 0)
        {
            take = 50;
        }

        var result =
            await service.ExecuteAsync(
                currentUser.UserId.Value,
                take,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/setup")]
    [Authorize]
    public async Task<IActionResult> SetupTwoFactor(
        [FromServices] SetupTwoFactorService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/enable")]
    [Authorize]
    public async Task<IActionResult> EnableTwoFactor(
        [FromBody] EnableTwoFactorRequest request,
        [FromServices] EnableTwoFactorService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                GetIpAddress(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/disable")]
    [Authorize]
    public async Task<IActionResult> DisableTwoFactor(
        [FromBody] DisableTwoFactorRequest request,
        [FromServices] DisableTwoFactorService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                GetIpAddress(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/login")]
    [AllowAnonymous]
    public async Task<IActionResult> TwoFactorLogin(
        [FromBody] TwoFactorLoginRequest request,
        [FromServices] TwoFactorLoginService service,
        CancellationToken cancellationToken)
    {
        var requestWithClientInfo = request with
        {
            IpAddress = GetIpAddress(),
            UserAgent = Request.Headers.UserAgent.ToString()
        };

        var result =
            await service.ExecuteAsync(
                requestWithClientInfo,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/recovery-codes")]
    [Authorize]
    public async Task<IActionResult> GenerateRecoveryCodes(
        [FromBody] GenerateRecoveryCodesRequest request,
        [FromServices] GenerateRecoveryCodesService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("2fa/regenerate-key")]
    [Authorize]
    public async Task<IActionResult> RegenerateAuthenticatorKey(
        [FromBody] RegenerateAuthenticatorKeyRequest request,
        [FromServices] RegenerateAuthenticatorKeyService service,
        CancellationToken cancellationToken)
    {
        var result =
            await service.ExecuteAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    private string? GetIpAddress()
    {
        return HttpContext
            .Connection
            .RemoteIpAddress?
            .ToString();
    }


}