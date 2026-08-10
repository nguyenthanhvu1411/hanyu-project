using HanYu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(
        EntityTypeBuilder<Role> builder)
    {
        builder.Property(
                x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(
                x => x.Code)
            .IsUnique();

        builder.Property(
                x => x.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(
                x => x.Description)
            .HasMaxLength(1000);

        builder.Property(
                x => x.IsSystem)
            .IsRequired();
    }
}
