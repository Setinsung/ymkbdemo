using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using YmKB.Application.Models;
using YmKB.Domain.Abstractions.Common;

namespace YmKB.Application.Extensions;

public static class QueryableExtensions
{
    #region OrderBy
    public static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> source,
        string orderByProperty
    ) => ApplyOrder(source, orderByProperty, "OrderBy");

    public static IOrderedQueryable<T> OrderByDescending<T>(
        this IQueryable<T> source,
        string orderByProperty
    ) => ApplyOrder(source, orderByProperty, "OrderByDescending");

    public static IOrderedQueryable<T> ThenBy<T>(
        this IOrderedQueryable<T> source,
        string orderByProperty
    ) => ApplyOrder(source, orderByProperty, "ThenBy");

    public static IOrderedQueryable<T> ThenByDescending<T>(
        this IOrderedQueryable<T> source,
        string orderByProperty
    ) => ApplyOrder(source, orderByProperty, "ThenByDescending");

    private static IOrderedQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        string property,
        string methodName
    )
    {
        var type = typeof(T);
        var propertyInfo = type.GetProperty(
            property,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
        );
        if (propertyInfo == null)
        {
            throw new ArgumentException(
                $"Property '{property}' does not exist on type '{type.Name}'."
            );
        }

        var parameter = Expression.Parameter(type, "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { type, propertyInfo.PropertyType },
            source.Expression,
            Expression.Quote(orderByExpression)
        );

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
    }

    public static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> source,
        string orderBy,
        string sortDirection
    )
    {
        return sortDirection.Equals("Descending", StringComparison.OrdinalIgnoreCase)
            ? source.OrderByDescending(orderBy)
            : source.OrderBy(orderBy);
    }
    #endregion
    public static async Task<PaginatedResult<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query,
        Expression<Func<T, bool>>? condition,
        int pageNumber,
        int pageSize,
        Func<T, TResult> mapperFunc,
        CancellationToken cancellationToken = default
    )
        where T : class, IEntity
    {
        if (condition != null)
        {
            query = (IOrderedQueryable<T>)query.Where(condition);
        }
        var count = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = data.Select(mapperFunc).ToList();
        return new PaginatedResult<TResult>(items, count, pageNumber, pageSize);
    }
}
