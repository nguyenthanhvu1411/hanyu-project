using HanYu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Configurations.Common;

public static class IdentityTableNameExtensions
{
    /// <summary>
    /// Đổi tên các bảng mặc định của ASP.NET Core Identity
    /// sang convention của HanYu.
    /// </summary>
    public static ModelBuilder ConfigureIdentityTableNames(
        this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("users");

        modelBuilder.Entity<Role>()
            .ToTable("roles");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");

        return modelBuilder;
    }
}

public static class PostgresValueConverters
{
    public static readonly Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<string?, System.Net.IPAddress?> NullableIpAddress =
        new(
            value => string.IsNullOrWhiteSpace(value) ? null : System.Net.IPAddress.Parse(value),
            value => value == null ? null : value.ToString());
}