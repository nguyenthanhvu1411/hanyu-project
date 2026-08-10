using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Sessions;

public sealed class GetSessionByIdHandler
{
    private readonly HanYuDbContext _dbContext;

    public GetSessionByIdHandler(HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<AdminSessionDto>> ExecuteAsync(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Set<UserSession>()
            .Include(x => x.User)
            .ThenInclude(x => x.Profile)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
        {
            return Result.Failure<AdminSessionDto>(Error.NotFound("Session.NotFound", "Không tìm thấy phiên đăng nhập."));
        }

        var dto = new AdminSessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            UserEmail = session.User.Email ?? string.Empty,
            UserDisplayName = session.User.Profile?.DisplayName ?? string.Empty,
            DeviceInfo = $"{session.DeviceName} {session.Browser} {session.OperatingSystem}".Trim(),
            IpAddress = session.IpAddress,
            UserAgent = session.UserAgent,
            CreatedAt = session.CreatedAt,
            LastUsedAt = session.LastActivityAt,
            ExpiresAt = null,
            RevokedAt = session.RevokedAt,
            RevokedReason = session.Status == HanYu.Domain.Enums.UserSessionStatus.Revoked ? "Thu hồi bởi Admin" : null,
            IsActive = session.Status == HanYu.Domain.Enums.UserSessionStatus.Active && !session.RevokedAt.HasValue
        };

        return Result.Success(dto);
    }
}
