namespace HanYu.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    long Total)
{
    public int TotalPages =>
        Total == 0
            ? 0
            : (int)Math.Ceiling(
                Total / (double)PageSize);

    public bool HasPrevious =>
        Page > 1;

    public bool HasNext =>
        Page < TotalPages;
}
