namespace HanYu.Application.Features.Identity.VerifyEmail;

public sealed record VerifyEmailRequest(
    Guid UserId,
    string Token);
