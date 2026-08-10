using System;

namespace HanYu.Domain.Constants;

public static class PaginationDefaults
{
    public const int DefaultPage =
        1;

    public const int DefaultPageSize =
        20;

    public const int MaxPageSize =
        100;

    public const int MinPageSize =
        1;

    public static int NormalizePage(
        int page)
    {
        return page < DefaultPage
            ? DefaultPage
            : page;
    }

    public static int NormalizePageSize(
        int pageSize)
    {
        return Math.Clamp(
            pageSize,
            MinPageSize,
            MaxPageSize);
    }
}
