namespace HanYu.Application.Features.Operations.Admin.SystemSettings;

public sealed record AdminSystemSettingResponse(
    long Id,
    string Key,
    string DisplayName,
    string Group,
    string Value,
    string ValueType,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record UpsertSystemSettingRequest(
    string Key,
    string DisplayName,
    string Group,
    string Value,
    string ValueType,
    string? Description);
