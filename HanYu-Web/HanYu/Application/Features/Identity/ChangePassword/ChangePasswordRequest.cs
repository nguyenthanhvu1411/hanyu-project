namespace HanYu.Application.Features.Identity.ChangePassword;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);
