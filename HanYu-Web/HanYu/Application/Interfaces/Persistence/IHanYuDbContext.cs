using Microsoft.EntityFrameworkCore;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Interfaces.Persistence;

public interface IHanYuDbContext
{
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>> RoleClaims { get; }

    DbSet<HanYu.Domain.Entities.Course.Course> Courses { get; }
    DbSet<HanYu.Domain.Entities.Course.CourseChapter> CourseChapters { get; }
    DbSet<HanYu.Domain.Entities.Course.CoursePrerequisite> CoursePrerequisites { get; }
    DbSet<HanYu.Domain.Entities.Vocabulary.HskLevel> HskLevels { get; }
    DbSet<HanYu.Domain.Entities.Lesson.Lesson> Lessons { get; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Add(object entity);
    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Remove(object entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}