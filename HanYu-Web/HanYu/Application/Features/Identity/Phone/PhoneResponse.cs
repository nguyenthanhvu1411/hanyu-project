namespace HanYu.Application.Features.Identity.Phone;

public sealed record PhoneResponse(
    string? PhoneNumber,
    bool PhoneNumberConfirmed);
