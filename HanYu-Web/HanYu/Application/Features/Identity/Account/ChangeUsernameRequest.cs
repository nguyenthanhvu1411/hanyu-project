namespace HanYu.Application.Features.Identity.Account;

public sealed record ChangeUsernameRequest(
    string NewUserName,
    string Password);
