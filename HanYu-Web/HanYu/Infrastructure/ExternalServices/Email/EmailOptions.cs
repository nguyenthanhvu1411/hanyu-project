namespace HanYu.Infrastructure.ExternalServices.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Host { get; init; } = string.Empty;

    public int Port { get; init; } = 587;

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string FromEmail { get; init; } = string.Empty;

    public string FromName { get; init; } = "HanYu";

    public bool UseSsl { get; init; }

    /// <summary>
    /// URL frontend public.
    /// Ví dụ:
    /// https://hanyu.vn
    /// hoặc local:
    /// http://localhost:3000
    /// </summary>
    public string FrontendBaseUrl { get; init; } = string.Empty;
}
