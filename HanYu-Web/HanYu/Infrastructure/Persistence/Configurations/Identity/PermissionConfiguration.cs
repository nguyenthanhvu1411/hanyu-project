using HanYu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Persistence.Configurations.Identity;

public sealed class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(
        EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(
            "permissions");

        builder.HasKey(
            x => x.Id);

        builder.Property(
                x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(
                x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(
                x => x.Code)
            .IsUnique();

        builder.Property(
                x => x.Resource)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(
                x => x.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(
                x => x.Description)
            .HasColumnType(
                "text");
    }
}
