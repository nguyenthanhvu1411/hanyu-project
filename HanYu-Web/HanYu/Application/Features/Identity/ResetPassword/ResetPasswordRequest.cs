namespace HanYu.Application.Features.Identity.ResetPassword;

public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword);
