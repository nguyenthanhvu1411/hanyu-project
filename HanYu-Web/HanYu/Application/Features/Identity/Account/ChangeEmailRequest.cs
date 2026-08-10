namespace HanYu.Application.Features.Identity.Account;

public sealed record ChangeEmailRequest(
    string NewEmail,
    string Password);
