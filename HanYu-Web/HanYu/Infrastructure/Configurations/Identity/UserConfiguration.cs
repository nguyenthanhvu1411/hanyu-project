using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HanYu.Infrastructure.Configurations.Identity;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("asp_net_users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.UserName)
            .HasMaxLength(100);

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(100);

        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(320);

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(320);

        builder.HasIndex(x => x.NormalizedEmail);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt).HasColumnType("timestamptz");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamptz");
        builder.Property(x => x.DeletedAt).HasColumnType("timestamptz");
        builder.Property(x => x.LastLoginAt).HasColumnType("timestamptz");

        builder.Ignore(x => x.IsDeleted);

        builder.HasOne(x => x.Profile)
            .WithOne(x => x.User)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Preference)
            .WithOne(x => x.User)
            .HasForeignKey<UserPreference>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
