using System.Text.Json;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.SecurityEvents;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class SecurityEventService
    : ISecurityEventService
{
    private const int MaxEvents = 100;

    private readonly HanYuDbContext _dbContext;

    public SecurityEventService(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task LogAsync(
        Guid userId,
        UserSecurityEventType eventType,
        string? ipAddress = null,
        string? userAgent = null,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        string? metadataJson = null;

        if (metadata is not null)
        {
            metadataJson =
                JsonSerializer.Serialize(
                    metadata);
        }

        var securityEvent =
            new UserSecurityEvent(
                userId,
                eventType,
                ipAddress,
                userAgent,
                metadataJson);

        _dbContext
            .Set<UserSecurityEvent>()
            .Add(securityEvent);

        // Không SaveChanges ở đây.
        // Caller có thể commit cùng transaction.

        return Task.CompletedTask;
    }

    public async Task<
        Result<IReadOnlyCollection<SecurityEventResponse>>>
        GetAsync(
            Guid userId,
            int take = 50,
            CancellationToken cancellationToken = default)
    {
        take =
            Math.Clamp(
                take,
                1,
                MaxEvents);

        var events =
            await _dbContext
                .Set<UserSecurityEvent>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .OrderByDescending(
                    x => x.OccurredAt)
                .Take(take)
                .Select(
                    x => new SecurityEventResponse(
                        x.EventType.ToString(),
                        x.IpAddress,
                        x.UserAgent,
                        x.MetadataJson,
                        x.OccurredAt))
                .ToListAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<SecurityEventResponse>>(
            events);
    }
}
