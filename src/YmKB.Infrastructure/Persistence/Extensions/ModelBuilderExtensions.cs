using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace YmKB.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// 为实现了指定接口的所有实体类型应用全局查询过滤器。
    /// 此方法遍历所有实体类型，查找实现了 `TInterface` 接口的实体，并为这些实体应用指定的查询过滤器表达式。
    /// </summary>
    /// <typeparam name="TInterface">要应用过滤器的实体必须实现的接口类型。</typeparam>
    /// <param name="modelBuilder">用于配置实体框架模型的 `ModelBuilder` 实例。</param>
    /// <param name="expression">表示要应用的查询过滤器的表达式。</param>
    public static void ApplyGlobalFilters<TInterface>(
        this ModelBuilder modelBuilder,
        Expression<Func<TInterface, bool>> expression
    )
    {
        // 从模型中获取所有实体类型，筛选出实现了 TInterface 接口的实体类型
        var entities = modelBuilder
            .Model
            .GetEntityTypes()
            .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
            .Select(e => e.ClrType);

        // 遍历筛选出的实体类型
        foreach (var entity in entities)
        {
            // 创建一个新的参数表达式，其类型为当前实体类型
            var newParam = Expression.Parameter(entity);

            // 替换表达式中的参数，使其与当前实体类型匹配
            var newBody = ReplacingExpressionVisitor.Replace(
                expression.Parameters.Single(),
                newParam,
                expression.Body
            );

            // 为当前实体类型应用查询过滤器
            modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newBody, newParam));
        }
    }
}