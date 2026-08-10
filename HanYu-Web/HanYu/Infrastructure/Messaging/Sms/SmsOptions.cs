namespace HanYu.Infrastructure.Messaging.Sms;

public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    public string AccountSid { get; init; } = string.Empty;

    public string AuthToken { get; init; } = string.Empty;

    public string FromNumber { get; init; } = string.Empty;

    public string? MessagingServiceSid { get; init; }

    public string ApplicationName { get; init; } = "HanYu";
}
