using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public sealed class IdentitySeeder
{
    private readonly HanYuDbContext _dbContext;

    private readonly UserManager<User> _userManager;

    private readonly RoleManager<Role> _roleManager;

    private readonly IdentitySeedOptions _options;

    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        HanYuDbContext dbContext,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<IdentitySeedOptions> options,
        ILogger<IdentitySeeder> logger)
    {
        _dbContext =
            dbContext;

        _userManager =
            userManager;

        _roleManager =
            roleManager;

        _options =
            options.Value;

        _logger =
            logger;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation(
                "Identity seeding is disabled.");

            return;
        }

        _logger.LogInformation(
            "Starting Identity seed...");

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _dbContext.Database
                    .BeginTransactionAsync(
                        cancellationToken);

            try
            {
                var permissions =
                    await SeedPermissionsAsync(
                        cancellationToken);

                var roles =
                    await SeedRolesAsync(
                        cancellationToken);

                await SeedRolePermissionsAsync(
                    roles,
                    permissions,
                    cancellationToken);

                await SeedSuperAdminAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Identity seed completed successfully.");
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                _logger.LogError(
                    exception,
                    "Identity seed failed.");

                throw;
            }
        });
    }

    // ============================================================
    // PERMISSIONS
    // ============================================================

    private async Task<Dictionary<string, Permission>>
        SeedPermissionsAsync(
            CancellationToken cancellationToken)
    {
        var catalog =
            IdentityPermissionSeedCatalog.All;

        var codes =
            catalog
                .Select(x => x.Code)
                .ToArray();

        var existingPermissions =
            await _dbContext.Permissions
                .IgnoreQueryFilters()
                .Where(x =>
                    codes.Contains(
                        x.Code))
                .ToListAsync(
                    cancellationToken);

        var map =
            existingPermissions
                .ToDictionary(
                    x => x.Code,
                    StringComparer.OrdinalIgnoreCase);

        foreach (var item in catalog)
        {
            if (map.TryGetValue(
                    item.Code,
                    out var existing))
            {
                UpdatePermission(
                    existing,
                    item);

                continue;
            }

            var permission =
                CreatePermission(
                    item);

            _dbContext.Permissions.Add(
                permission);

            map[item.Code] =
                permission;
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Seeded {Count} permissions.",
            map.Count);

        return map;
    }

    // ============================================================
    // ROLES
    // ============================================================

    private async Task<Dictionary<string, Role>>
        SeedRolesAsync(
            CancellationToken cancellationToken)
    {
        var result =
            new Dictionary<string, Role>(
                StringComparer.OrdinalIgnoreCase);

        foreach (
            var item
            in IdentityRoleSeedCatalog.All)
        {
            var role =
                await _roleManager.FindByNameAsync(
                    item.Code);

            if (role is null)
            {
                role =
                    new Role(
                        code:
                            item.Code,

                        displayName:
                            item.Name,

                        description:
                            item.Description,

                        isSystem:
                            true);

                var createResult =
                    await _roleManager.CreateAsync(
                        role);

                ThrowIfFailed(
                    createResult,
                    $"Không thể tạo role '{item.Code}'.");

                _logger.LogInformation(
                    "Created role {Role}.",
                    item.Code);
            }
            else
            {
                var changed =
                    false;

                if (!string.Equals(
                        role.DisplayName,
                        item.Name,
                        StringComparison.Ordinal))
                {
                    role.Rename(
                        item.Name);

                    changed =
                        true;
                }

                if (!string.Equals(
                        role.Description,
                        item.Description,
                        StringComparison.Ordinal))
                {
                    role.UpdateDescription(
                        item.Description);

                    changed =
                        true;
                }

                if (changed)
                {
                    var updateResult =
                        await _roleManager.UpdateAsync(
                            role);

                    ThrowIfFailed(
                        updateResult,
                        $"Không thể cập nhật role '{item.Code}'.");
                }
            }

            result[item.Code] =
                role;
        }

        return result;
    }

    // ============================================================
    // ROLE PERMISSIONS
    // ============================================================

    private async Task SeedRolePermissionsAsync(
        IReadOnlyDictionary<string, Role> roles,
        IReadOnlyDictionary<string, Permission> permissions,
        CancellationToken cancellationToken)
    {
        foreach (
            var roleSeed
            in IdentityRoleSeedCatalog.All)
        {
            if (!roles.TryGetValue(
                    roleSeed.Code,
                    out var role))
            {
                throw new InvalidOperationException(
                    $"Role '{roleSeed.Code}' không tồn tại.");
            }

            var desiredPermissionCodes =
                roleSeed.Permissions
                    .Where(
                        permissions.ContainsKey)
                    .ToHashSet();

            var currentClaims =
                await _roleManager.GetClaimsAsync(
                    role);

            var currentPermissionCodes =
                currentClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToHashSet();

            // =========================
            // REMOVE
            // =========================

            var currentClaimsList = await _dbContext.RoleClaims
                .Where(c => c.RoleId == role.Id && c.ClaimType == "Permission")
                .ToListAsync(cancellationToken);

            var toRemove = currentClaimsList
                .Where(c => !desiredPermissionCodes.Contains(c.ClaimValue ?? string.Empty))
                .ToList();

            if (toRemove.Any())
            {
                _dbContext.RoleClaims.RemoveRange(toRemove);
            }

            // =========================
            // ADD
            // =========================

            var missing =
                desiredPermissionCodes
                    .Where(code => !currentPermissionCodes.Contains(code))
                    .ToArray();

            if (missing.Any())
            {
                var newClaims = missing.Select(code => new Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = "Permission",
                    ClaimValue = code
                });
                await _dbContext.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "Role {Role} has {PermissionCount} permissions.",
                roleSeed.Code,
                desiredPermissionCodes.Count);
        }
    }

    // ============================================================
    // SUPER ADMIN
    // ============================================================

    private async Task SeedSuperAdminAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                _options.SuperAdminEmail))
        {
            _logger.LogWarning(
                "IdentitySeed:SuperAdminEmail is empty. " +
                "SuperAdmin account will not be created.");

            return;
        }

        if (string.IsNullOrWhiteSpace(
                _options.SuperAdminPassword))
        {
            _logger.LogWarning(
                "IdentitySeed:SuperAdminPassword is empty. " +
                "SuperAdmin account will not be created.");

            return;
        }

        var email =
            _options.SuperAdminEmail
                .Trim()
                .ToLowerInvariant();

        var userName =
            string.IsNullOrWhiteSpace(
                _options.SuperAdminUserName)
                ? email
                : _options.SuperAdminUserName.Trim();

        var user =
            await _userManager.FindByNameAsync(
                userName);

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(email);
        }

        if (user is null)
        {
            user =
                new User(
                    userName,
                    email);

            var result =
                await _userManager.CreateAsync(
                    user,
                    _options.SuperAdminPassword);

            ThrowIfFailed(
                result,
                "Không thể tạo tài khoản SUPER_ADMIN.");

            _logger.LogInformation(
                "Created SUPER_ADMIN account {Email}.",
                email);
        }

        // =========================
        // CONFIRM EMAIL
        // =========================

        if (!user.EmailConfirmed)
        {
            user.ConfirmEmail();

            var updateResult =
                await _userManager.UpdateAsync(
                    user);

            ThrowIfFailed(
                updateResult,
                "Không thể xác minh email SUPER_ADMIN.");
        }

        // =========================
        // ADD SUPER_ADMIN ROLE
        // =========================

        if (!await _userManager.IsInRoleAsync(
                user,
                RoleCodes.SuperAdmin))
        {
            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    RoleCodes.SuperAdmin);

            ThrowIfFailed(
                roleResult,
                "Không thể gán SUPER_ADMIN role.");
        }

        // =========================
        // PROFILE
        // =========================

        await EnsureSuperAdminProfileAsync(
            user,
            cancellationToken);

        _logger.LogInformation(
            "SUPER_ADMIN seed completed for {Email}.",
            email);
    }

    private async Task EnsureSuperAdminProfileAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var profileExists =
            await _dbContext.UserProfiles
                .AnyAsync(
                    x =>
                        x.UserId ==
                        user.Id,
                    cancellationToken);

        if (profileExists)
        {
            return;
        }

        var displayName =
            string.IsNullOrWhiteSpace(
                _options.SuperAdminDisplayName)
                ? "HanYu Super Administrator"
                : _options.SuperAdminDisplayName.Trim();

        var profile =
            new UserProfile(
                user.Id,
                displayName);

        _dbContext.UserProfiles.Add(
            profile);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    // ============================================================
    // ENTITY FACTORIES
    // ============================================================

    private static Permission CreatePermission(
        PermissionSeedItem item)
    {
        return new Permission(
            item.Code,
            item.Resource,
            item.Action,
            item.Description);
    }

    private static void UpdatePermission(
        Permission permission,
        PermissionSeedItem item)
    {
        permission.Update(
            resource:
                item.Resource,
            action:
                item.Action,
            description:
                item.Description);
    }

    private static RolePermission CreateRolePermission(
        Guid roleId,
        long permissionId)
    {
        return new RolePermission(
            roleId,
            permissionId);
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static void ThrowIfFailed(
        IdentityResult result,
        string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors =
            string.Join(
                "; ",
                result.Errors.Select(
                    x =>
                        $"{x.Code}: {x.Description}"));

        throw new InvalidOperationException(
            $"{message} {errors}");
    }
}
