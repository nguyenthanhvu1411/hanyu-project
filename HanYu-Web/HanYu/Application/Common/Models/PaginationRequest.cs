using HanYu.Domain.Constants;

namespace HanYu.Application.Common.Models;

public abstract record PaginationRequest
{
    public int Page { get; init; } =
        PaginationDefaults.DefaultPage;

    public int PageSize { get; init; } =
        PaginationDefaults.DefaultPageSize;

    public int NormalizedPage =>
        PaginationDefaults.NormalizePage(
            Page);

    public int NormalizedPageSize =>
        PaginationDefaults.NormalizePageSize(
            PageSize);
}
