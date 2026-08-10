using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Configurations.Common;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Apply toàn bộ IEntityTypeConfiguration&lt;TEntity&gt;
    /// trong assembly HanYu.Infrastructure.
    /// </summary>
    public static ModelBuilder ApplyHanYuConfigurations(
        this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        return modelBuilder;
    }
}