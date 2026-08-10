using HanYu.Application.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HanYu.Infrastructure.Messaging.Sms;

public sealed class TwilioSmsService : ISmsService
{
    private readonly SmsOptions _options;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(
        IOptions<SmsOptions> options,
        ILogger<TwilioSmsService> logger)
    {
        _options = options.Value;
        _logger = logger;

        TwilioClient.Init(
            _options.AccountSid,
            _options.AuthToken);
    }

    public async Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException(
                "Phone number không được để trống.",
                nameof(phoneNumber));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Verification code không được để trống.",
                nameof(code));
        }

        var body =
            $"{_options.ApplicationName}: " +
            $"Mã xác minh của bạn là {code}. " +
            "Không chia sẻ mã này với bất kỳ ai.";

        MessageResource message;

        if (!string.IsNullOrWhiteSpace(
                _options.MessagingServiceSid))
        {
            message =
                await MessageResource.CreateAsync(
                    to: new PhoneNumber(phoneNumber),
                    body: body,
                    messagingServiceSid:
                        _options.MessagingServiceSid);
        }
        else
        {
            message =
                await MessageResource.CreateAsync(
                    to: new PhoneNumber(phoneNumber),
                    from: new PhoneNumber(
                        _options.FromNumber),
                    body: body);
        }

        _logger.LogInformation(
            "Phone verification SMS queued. Sid={MessageSid}",
            message.Sid);
    }
}
