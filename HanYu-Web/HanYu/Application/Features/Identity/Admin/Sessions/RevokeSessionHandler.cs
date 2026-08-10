using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Application.Features.Identity.Admin.Sessions;

public sealed class RevokeSessionHandler
{
    private readonly HanYuDbContext _dbContext;

    public RevokeSessionHandler(HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> ExecuteAsync(long sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Set<UserSession>()
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
        {
            return Result.Failure(Error.NotFound("Session.NotFound", "Không tìm thấy phiên đăng nhập."));
        }

        if (session.IsActive)
        {
            session.Revoke();
            
            // Note: We might also want to revoke related RefreshTokens
            var relatedTokens = await _dbContext.Set<HanYu.Domain.Entities.Identity.RefreshToken>()
                .Where(x => x.UserSessionId == session.Id && x.RevokedAt == null)
                .ToListAsync(cancellationToken);
                
            foreach (var token in relatedTokens)
            {
                token.Revoke(null, "Admin revoked session");
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
