using HanYu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Persistence.Configurations.Identity;

public sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(
        EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(
            "role_permissions");

        builder.HasKey(
            x => new
            {
                x.RoleId,
                x.PermissionId
            });

        builder.HasOne(
                x => x.Role)
            .WithMany()
            .HasForeignKey(
                x => x.RoleId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.HasOne(
                x => x.Permission)
            .WithMany()
            .HasForeignKey(
                x => x.PermissionId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.HasIndex(
            x => x.PermissionId);
    }
}
