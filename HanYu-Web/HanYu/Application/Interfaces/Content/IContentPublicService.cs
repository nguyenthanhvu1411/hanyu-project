using HanYu.Application.Common.Models;
using HanYu.Application.Features.Content.Public.Reports;

namespace HanYu.Application.Interfaces.Content;

public interface IContentPublicService
{
    Task<Result<MyContentReportResponse>>
        CreateReportAsync(
            Guid userId,
            CreateContentReportRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<MyContentReportResponse>>>
        GetMyReportsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result> UpdateMyReportAsync(
        Guid userId,
        Guid publicId,
        string? description,
        CancellationToken cancellationToken = default);
}
