using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HanYu.Application.Features.Identity.Admin.Sessions;

public sealed class GetSessionsQuery
{
    public string? Search { get; init; }
    public bool? Active { get; init; }
    public Guid? UserId { get; init; }
    public string? IpAddress { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class GetSessionsHandler
{
    private readonly HanYuDbContext _dbContext;

    public GetSessionsHandler(HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<AdminSessionDto>>> ExecuteAsync(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<UserSession>()
            .Include(x => x.User)
            .ThenInclude(x => x.Profile)
            .AsNoTracking();

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId.Value);
        }

        if (request.Active.HasValue)
        {
            if (request.Active.Value)
            {
                query = query.Where(x => x.Status == HanYu.Domain.Enums.UserSessionStatus.Active && !x.RevokedAt.HasValue);
            }
            else
            {
                query = query.Where(x => x.Status != HanYu.Domain.Enums.UserSessionStatus.Active || x.RevokedAt.HasValue);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.IpAddress))
        {
            var ip = request.IpAddress.Trim();
            query = query.Where(x => x.IpAddress != null && x.IpAddress.Contains(ip));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(x => 
                (x.User.UserName != null && x.User.UserName.ToLower().Contains(term)) ||
                (x.User.Email != null && x.User.Email.ToLower().Contains(term)) ||
                (x.IpAddress != null && x.IpAddress.Contains(term)) ||
                (x.DeviceName != null && x.DeviceName.ToLower().Contains(term))
            );
        }

        var total = await query.CountAsync(cancellationToken);

        var sessions = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = sessions.Select(s => new AdminSessionDto
        {
            Id = s.Id,
            UserId = s.UserId,
            UserEmail = s.User.Email ?? string.Empty,
            UserDisplayName = s.User.Profile?.DisplayName ?? string.Empty,
            DeviceInfo = $"{s.DeviceName} {s.Browser} {s.OperatingSystem}".Trim(),
            IpAddress = s.IpAddress,
            UserAgent = s.UserAgent,
            CreatedAt = s.CreatedAt,
            LastUsedAt = s.LastActivityAt,
            ExpiresAt = null, // Adjust if UserSession tracks ExpiresAt
            RevokedAt = s.RevokedAt,
            RevokedReason = s.Status == HanYu.Domain.Enums.UserSessionStatus.Revoked ? "Thu hồi bởi Admin" : null,
            IsActive = s.Status == HanYu.Domain.Enums.UserSessionStatus.Active && !s.RevokedAt.HasValue
        }).ToList();

        var result = new PagedResult<AdminSessionDto>(dtos, request.Page, request.PageSize, total);

        return Result<PagedResult<AdminSessionDto>>.Success(result);
    }
}
