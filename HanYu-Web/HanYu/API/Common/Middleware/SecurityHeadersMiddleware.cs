namespace HanYu.API.Common.Middleware;

/// <summary>
/// Adds production security headers to every HTTP response.
///
/// Headers applied:
///   X-Content-Type-Options: nosniff         — Prevents MIME sniffing attacks
///   X-Frame-Options: DENY                   — Blocks clickjacking via iframe
///   Referrer-Policy: ...                    — Limits referrer info leakage
///   Permissions-Policy: ...                 — Disables unnecessary browser features
///   X-XSS-Protection: 0                     — Tells modern browsers to use CSP instead
///
/// CSP is intentionally NOT hardcoded here.
/// CSP must be designed per frontend domain and should be configured externally.
/// A wrong CSP can break the entire application.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent MIME sniffing (e.g., serving JS as HTML)
        headers["X-Content-Type-Options"] = "nosniff";

        // Block rendering in iframes to prevent clickjacking
        headers["X-Frame-Options"] = "DENY";

        // Limit Referer header leakage to same-origin and cross-origin safe subset
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Disable sensitive browser features not needed for an API
        headers["Permissions-Policy"] =
            "camera=(), microphone=(), geolocation=(), payment=(), usb=()";

        // Disable legacy XSS filter (deprecated; modern browsers use CSP instead)
        // Setting to 0 avoids a known bypass where filter mode=block enables XSS
        headers["X-XSS-Protection"] = "0";

        // Remove server identification headers to reduce information leakage
        headers.Remove("Server");
        headers.Remove("X-Powered-By");
        headers.Remove("X-AspNet-Version");

        await _next(context);
    }
}
