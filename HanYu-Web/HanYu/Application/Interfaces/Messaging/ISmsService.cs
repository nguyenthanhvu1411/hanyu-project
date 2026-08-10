namespace HanYu.Application.Interfaces.Messaging;

public interface ISmsService
{
    Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);
}
