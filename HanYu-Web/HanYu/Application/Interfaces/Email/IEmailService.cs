namespace HanYu.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendVerificationEmailAsync(
        string email,
        string displayName,
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    Task SendResetPasswordEmailAsync(
        string email,
        string displayName,
        string token,
        CancellationToken cancellationToken = default);
}
