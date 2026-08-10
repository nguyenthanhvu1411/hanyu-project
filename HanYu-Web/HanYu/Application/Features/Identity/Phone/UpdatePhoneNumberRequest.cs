namespace HanYu.Application.Features.Identity.Phone;

public sealed record UpdatePhoneNumberRequest(
    string PhoneNumber,
    string Password);
