using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using YmKB.Domain.Abstractions.Common;

namespace YmKB.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 该拦截器用于在数据库保存更改时调度域事件。
/// 它会拦截数据库保存更改的操作，检测实体中的域事件，
/// 并使用中介者模式将这些域事件发布出去，同时管理事务以确保数据一致性。
/// </summary>
public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 在数据库保存更改之前被调用，用于处理删除状态实体的域事件。
    /// 此方法会找出所有已删除且包含域事件的实体，提取这些实体的域事件，
    /// 并在保存更改后使用中介者发布这些事件。整个过程在事务中进行，以确保数据一致性。
    /// </summary>
    /// <param name="eventData">包含数据库上下文等相关信息的事件数据。</param>
    /// <param name="result">拦截操作的结果。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="ValueTask{TResult}"/>，结果为拦截操作的结果。</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        // 获取当前的数据库上下文
        var context = eventData.Context;
        // 如果上下文为空，直接调用基类的方法
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        // 找出所有已删除且包含域事件的实体
        var domainEventEntities = context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count != 0 && e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        // 提取这些实体的所有域事件
        var domainEvents = domainEventEntities.SelectMany(e => e.DomainEvents).ToList();

        // 如果存在域事件
        if (domainEvents.Count != 0)
        {
            // 开始一个事务
            await using var transaction = await context
                .Database
                .BeginTransactionAsync(cancellationToken);
            try
            {
                // 调用基类的保存更改方法
                var saveResult = await base.SavingChangesAsync(
                    eventData,
                    result,
                    cancellationToken
                );

                // 清除实体中的域事件
                domainEventEntities.ForEach(e => e.ClearDomainEvents());
                // 发布每个域事件
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                // 提交事务
                await transaction.CommitAsync(cancellationToken);
                return saveResult;
            }
            catch
            {
                // 发生异常时回滚事务
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // 如果没有域事件，直接调用基类的方法
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 在数据库保存更改完成后被调用，用于处理非删除状态实体的域事件。
    /// 此方法会找出所有非删除状态且包含域事件的实体，提取这些实体的域事件，
    /// 并在保存更改后使用中介者发布这些事件。整个过程在事务中进行，以确保数据一致性。
    /// </summary>
    /// <param name="eventData">包含数据库上下文等相关信息的事件数据。</param>
    /// <param name="result">保存更改操作的结果。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="ValueTask{TResult}"/>，结果为保存更改操作的结果。</returns>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        // 获取当前的数据库上下文
        var context = eventData.Context;
        // 如果上下文为空，直接调用基类的方法
        if (context == null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        // 找出所有非删除状态且包含域事件的实体
        var domainEventEntities = context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count != 0 && e.State != EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        // 提取这些实体的所有域事件
        var domainEvents = domainEventEntities.SelectMany(e => e.DomainEvents).ToList();

        // 如果存在域事件
        if (domainEvents.Count != 0)
        {
            // 开始一个事务
            await using var transaction = await context
                .Database
                .BeginTransactionAsync(cancellationToken);
            try
            {
                // 调用基类的保存更改完成方法
                var saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken);

                // 清除实体中的域事件
                domainEventEntities.ForEach(e => e.ClearDomainEvents());
                // 发布每个域事件
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                // 提交事务
                await transaction.CommitAsync(cancellationToken);
                return saveResult;
            }
            catch
            {
                // 发生异常时回滚事务
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // 如果没有域事件，直接调用基类的方法
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
