using HanYu.Application.Common.Models;
using HanYu.Application.Features.Operations.Admin.AuditLogs;
using HanYu.Application.Features.Operations.Admin.ProductEvents;

namespace HanYu.Application.Interfaces.Operations;

public interface IOperationsAdminService
{
    Task<Result<PagedResult<AdminAuditLogResponse>>>
        GetAuditLogsAsync(
            AdminAuditLogQuery query,
            CancellationToken cancellationToken = default);

    Task<Result<AdminAuditLogResponse>>
        GetAuditLogAsync(
            long id,
            CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminProductEventResponse>>>
        GetProductEventsAsync(
            AdminProductEventQuery query,
            CancellationToken cancellationToken = default);
}
