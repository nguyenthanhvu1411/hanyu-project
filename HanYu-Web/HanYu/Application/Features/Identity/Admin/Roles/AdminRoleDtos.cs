namespace HanYu.Application.Features.Identity.Admin.Roles;

public record AdminRoleListItemDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsSystem { get; init; }
    public int UserCount { get; init; }
    public int PermissionCount { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public string? ConcurrencyToken { get; init; }
}

public sealed record AdminRoleDetailDto : AdminRoleListItemDto
{
    public List<AdminRolePermissionDto> Permissions { get; init; } = [];
}

public sealed record AdminRolePermissionDto
{
    public string Id { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Resource { get; init; }
    public string? Action { get; init; }
}
