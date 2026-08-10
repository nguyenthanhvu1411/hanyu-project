using HanYu.Application.Interfaces.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace HanYu.Infrastructure.ExternalServices.Email;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailOptions> options,
        ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(
        string email,
        string displayName,
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var verificationUrl =
            BuildVerificationUrl(
                userId,
                token);

        var subject =
            "Xác minh tài khoản HanYu";

        var body = $"""
            <!DOCTYPE html>
            <html lang="vi">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport"
                      content="width=device-width, initial-scale=1.0">
            </head>

            <body style="
                margin:0;
                padding:0;
                background:#f5f5f5;
                font-family:Arial,Helvetica,sans-serif;
            ">

                <div style="
                    max-width:600px;
                    margin:40px auto;
                    background:#ffffff;
                    padding:32px;
                    border-radius:12px;
                ">

                    <h1 style="
                        margin-top:0;
                        color:#111827;
                    ">
                        Chào {HtmlEncode(displayName)} 👋
                    </h1>

                    <p style="
                        color:#4b5563;
                        font-size:16px;
                        line-height:1.6;
                    ">
                        Cảm ơn bạn đã đăng ký tài khoản HanYu.
                    </p>

                    <p style="
                        color:#4b5563;
                        font-size:16px;
                        line-height:1.6;
                    ">
                        Vui lòng xác minh địa chỉ email của bạn
                        bằng cách nhấn vào nút bên dưới.
                    </p>

                    <div style="
                        margin:32px 0;
                        text-align:center;
                    ">

                        <a
                            href="{verificationUrl}"
                            style="
                                display:inline-block;
                                padding:14px 28px;
                                background:#111827;
                                color:#ffffff;
                                text-decoration:none;
                                border-radius:8px;
                                font-weight:bold;
                            ">
                            Xác minh email
                        </a>

                    </div>

                    <p style="
                        color:#6b7280;
                        font-size:14px;
                    ">
                        Nếu bạn không tạo tài khoản HanYu,
                        bạn có thể bỏ qua email này.
                    </p>

                </div>

            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            displayName,
            subject,
            body,
            cancellationToken);
    }

    public async Task SendResetPasswordEmailAsync(
        string email,
        string displayName,
        string token,
        CancellationToken cancellationToken = default)
    {
        var resetUrl =
            BuildResetPasswordUrl(
                email,
                token);

        var subject =
            "Đặt lại mật khẩu HanYu";

        var body = $"""
            <!DOCTYPE html>
            <html lang="vi">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport"
                      content="width=device-width, initial-scale=1.0">
            </head>

            <body style="
                margin:0;
                padding:0;
                background:#f5f5f5;
                font-family:Arial,Helvetica,sans-serif;
            ">

                <div style="
                    max-width:600px;
                    margin:40px auto;
                    background:#ffffff;
                    padding:32px;
                    border-radius:12px;
                ">

                    <h1 style="
                        margin-top:0;
                        color:#111827;
                    ">
                        Đặt lại mật khẩu
                    </h1>

                    <p style="
                        color:#4b5563;
                        font-size:16px;
                        line-height:1.6;
                    ">
                        Chào {HtmlEncode(displayName)},
                    </p>

                    <p style="
                        color:#4b5563;
                        font-size:16px;
                        line-height:1.6;
                    ">
                        HanYu nhận được yêu cầu đặt lại
                        mật khẩu cho tài khoản của bạn.
                    </p>

                    <div style="
                        margin:32px 0;
                        text-align:center;
                    ">

                        <a
                            href="{resetUrl}"
                            style="
                                display:inline-block;
                                padding:14px 28px;
                                background:#111827;
                                color:#ffffff;
                                text-decoration:none;
                                border-radius:8px;
                                font-weight:bold;
                            ">
                            Đặt lại mật khẩu
                        </a>

                    </div>

                    <p style="
                        color:#6b7280;
                        font-size:14px;
                    ">
                        Nếu bạn không yêu cầu thay đổi
                        mật khẩu, hãy bỏ qua email này.
                    </p>

                </div>

            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            displayName,
            subject,
            body,
            cancellationToken);
    }

    private async Task SendEmailAsync(
        string email,
        string recipientName,
        string subject,
        string html,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _options.FromName,
                _options.FromEmail));

        message.To.Add(
            new MailboxAddress(
                recipientName,
                email));

        message.Subject = subject;

        message.Body =
            new TextPart(TextFormat.Html)
            {
                Text = html
            };

        using var client =
            new SmtpClient();

        try
        {
            var socketOptions =
                _options.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                socketOptions,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(
                    _options.UserName))
            {
                await client.AuthenticateAsync(
                    _options.UserName,
                    _options.Password,
                    cancellationToken);
            }

            await client.SendAsync(
                message,
                cancellationToken);

            await client.DisconnectAsync(
                true,
                cancellationToken);

            _logger.LogInformation(
                "Email {Subject} sent to {Email}",
                subject,
                email);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed sending email {Subject} to {Email}",
                subject,
                email);

            throw;
        }
    }

    private string BuildVerificationUrl(
        Guid userId,
        string token)
    {
        var baseUrl =
            _options.FrontendBaseUrl
                .TrimEnd('/');

        return
            $"{baseUrl}/verify-email" +
            $"?userId={Uri.EscapeDataString(userId.ToString())}" +
            $"&token={Uri.EscapeDataString(token)}";
    }

    private string BuildResetPasswordUrl(
        string email,
        string token)
    {
        var baseUrl =
            _options.FrontendBaseUrl
                .TrimEnd('/');

        return
            $"{baseUrl}/reset-password" +
            $"?email={Uri.EscapeDataString(email)}" +
            $"&token={Uri.EscapeDataString(token)}";
    }

    private static string HtmlEncode(
        string value)
    {
        return System.Net.WebUtility.HtmlEncode(
            value);
    }
}
