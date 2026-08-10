using HanYu.Application.Common.Models;
using HanYu.Application.Features.Content.Admin.Imports;
using HanYu.Application.Features.Content.Admin.Reports;

namespace HanYu.Application.Interfaces.Content;

public interface IContentAdminService
{
    Task<Result<PagedResult<AdminContentImportJobResponse>>>
        GetImportJobsAsync(
            AdminContentImportJobQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminContentImportJobResponse>>
        GetImportJobAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<AdminContentImportJobResponse>>
        CreateImportJobAsync(
            CreateContentImportJobRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<AdminContentImportJobResponse>>
        UpdateImportSourceAsync(
            long id,
            UpdateContentImportSourceRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AdminContentImportRowResponse>>>
        GetImportRowsAsync(
            long importJobId,
            CancellationToken cancellationToken = default);

    Task<Result> DeleteImportJobAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminContentReportResponse>>>
        GetReportsAsync(
            AdminContentReportQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminContentReportResponse>>
        GetReportAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result> StartReportReviewAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> ResolveReportAsync(
        long id,
        Guid adminUserId,
        ResolveContentReportRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> RejectReportAsync(
        long id,
        Guid adminUserId,
        ResolveContentReportRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ReopenReportAsync(
        long id,
        CancellationToken cancellationToken = default);
}
