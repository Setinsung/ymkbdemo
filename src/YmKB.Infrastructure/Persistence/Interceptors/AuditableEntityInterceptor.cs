using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using YmKB.Application.Contracts;
using YmKB.Application.Contracts.Identity;
using YmKB.Domain.Abstractions.Common;
using YmKB.Domain.Abstractions.SystemEntities;
using YmKB.Infrastructure.Persistence.Extensions;

namespace YmKB.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 用于拦截数据库保存更改操作的审计实体拦截器。
/// 该拦截器主要用于处理可审计实体的审计信息更新，生成审计跟踪记录，并在保存成功或失败时进行相应处理。
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IDateTime _dateTime;
    private List<AuditTrail> _temporaryAuditTrailList =  [ ];

    public AuditableEntityInterceptor(IServiceProvider serviceProvider, IDateTime dateTime)
    {
        _currentUserAccessor = serviceProvider.GetRequiredService<ICurrentUserAccessor>();
        _dbContextFactory = serviceProvider.GetRequiredService<
            IDbContextFactory<ApplicationDbContext>
        >();
        _dateTime = dateTime;
    }

    /// <summary>
    /// 在数据库保存更改操作执行之前被调用。
    /// 此方法用于更新可审计实体的审计信息，并生成审计跟踪记录。
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
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        // 更新可审计实体的审计信息
        UpdateAuditableEntities(context);
        // 生成审计跟踪记录
        _temporaryAuditTrailList = GenerateAuditTrails(context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// 在数据库保存更改操作完成之后被调用。
    /// 此方法用于完成审计跟踪记录的最终处理并将其保存到数据库中。
    /// </summary>
    /// <param name="eventData">包含数据库上下文等相关信息的保存更改完成事件数据。</param>
    /// <param name="result">保存更改操作的结果。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="ValueTask{TResult}"/>，结果为保存更改操作的结果。</returns>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        var context = eventData.Context;
        var saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken);
        if (context != null)
        {
            // 完成审计跟踪记录的最终处理并保存到数据库
            await FinalizeAuditTrailsAsync(context, cancellationToken);
        }
        return saveResult;
    }

    /// <summary>
    /// 当数据库保存更改操作失败时被调用。
    /// 此方法用于为临时审计跟踪记录设置错误信息，并使用新的数据库上下文保存这些记录。
    /// </summary>
    /// <param name="eventData">包含数据库上下文和异常信息的保存更改失败事件数据。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public override async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        await base.SaveChangesFailedAsync(eventData, cancellationToken);
        var context = eventData.Context;
        var exception = eventData.Exception;
        if (context != null)
        {
            var errorMessage =
                exception.InnerException != null
                    ? exception.InnerException.Message
                    : exception.Message;
            // 为临时审计跟踪记录设置错误信息
            foreach (var auditTrail in _temporaryAuditTrailList)
            {
                auditTrail.ErrorMessage = errorMessage;
            }
            // 使用新的数据库上下文保存审计跟踪记录
            await SaveAuditTrailsWithNewContextAsync(_temporaryAuditTrailList, cancellationToken);
        }
    }

    /// <summary>
    /// 更新可审计实体的审计信息，如创建者、创建时间、修改者、修改时间、删除者、删除时间等。
    /// 根据实体的状态（新增、修改、删除）来设置相应的审计信息。
    /// </summary>
    /// <param name="context">数据库上下文实例。</param>
    private void UpdateAuditableEntities(DbContext context)
    {
        var userId = _currentUserAccessor.UserId;
        var tenantId = _currentUserAccessor.TenantId;
        var now = _dateTime.Now;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // 设置新增实体的创建审计信息
                    SetCreationAuditInfo(entry.Entity, userId, tenantId, now);
                    break;

                case EntityState.Modified:
                    // 设置修改实体的修改审计信息
                    SetModificationAuditInfo(entry.Entity, userId, now);
                    break;

                case EntityState.Deleted:
                    // 设置删除实体的删除审计信息（如果是软删除）
                    SetDeletionAuditInfo(entry, userId, now);
                    break;

                case EntityState.Unchanged when entry.HasChangedOwnedEntities():
                    // 处理拥有实体更改的情况，设置修改审计信息
                    SetModificationAuditInfo(entry.Entity, userId, now);
                    break;
            }
        }
    }

    /// <summary>
    /// 设置新增可审计实体的创建审计信息，包括创建者、创建时间和租户 ID（如果适用）。
    /// </summary>
    /// <param name="entity">要设置审计信息的可审计实体。</param>
    /// <param name="userId">当前用户 ID。</param>
    /// <param name="tenantId">当前租户 ID。</param>
    /// <param name="now">当前日期和时间。</param>
    private static void SetCreationAuditInfo(
        IAuditableEntity entity,
        string userId,
        string tenantId,
        DateTime now
    )
    {
        entity.CreatedBy = userId;
        entity.Created = now;
        switch (entity)
        {
            case IMustHaveTenant mustTenant:
                mustTenant.TenantId = tenantId;
                break;
            case IMayHaveTenant mayTenant:
                mayTenant.TenantId = tenantId;
                break;
        }
    }

    /// <summary>
    /// 设置修改可审计实体的修改审计信息，包括修改者和修改时间。
    /// </summary>
    /// <param name="entity">要设置审计信息的可审计实体。</param>
    /// <param name="userId">当前用户 ID。</param>
    /// <param name="now">当前日期和时间。</param>
    private static void SetModificationAuditInfo(
        IAuditableEntity entity,
        string userId,
        DateTime now
    )
    {
        entity.LastModifiedBy = userId;
        entity.LastModified = now;
    }

    /// <summary>
    /// 设置删除可审计实体的删除审计信息（如果是软删除），包括删除者和删除时间，并将实体状态设置为修改。
    /// </summary>
    /// <param name="entry">实体条目。</param>
    /// <param name="userId">当前用户 ID。</param>
    /// <param name="now">当前日期和时间。</param>
    private static void SetDeletionAuditInfo(EntityEntry entry, string userId, DateTime now)
    {
        if (entry.Entity is ISoftDelete softDelete)
        {
            softDelete.DeletedBy = userId;
            softDelete.Deleted = now;
            entry.State = EntityState.Modified;
        }
    }

    /// <summary>
    /// 生成审计跟踪记录，遍历数据库上下文中符合条件的实体条目，为每个条目创建相应的审计跟踪记录。
    /// </summary>
    /// <param name="context">数据库上下文实例。</param>
    /// <returns>生成的审计跟踪记录列表。</returns>
    private List<AuditTrail> GenerateAuditTrails(DbContext context)
    {
        var userId = _currentUserAccessor.UserId;
        var now = _dateTime.Now;
        var auditTrails = new List<AuditTrail>();

        foreach (var entry in context.ChangeTracker.Entries<IAuditTrial>())
        {
            if (IsValidAuditEntry(entry))
            {
                var auditTrail = CreateAuditTrail(entry, userId, now, entry.DebugView.LongView);
                auditTrails.Add(auditTrail);
            }
        }

        return auditTrails;
    }

    /// <summary>
    /// 判断给定的实体条目是否为有效的审计条目。
    /// 有效的审计条目需满足：实体不是审计跟踪记录本身，并且实体状态不是分离状态或未更改状态。
    /// </summary>
    /// <param name="entry">要检查的实体条目。</param>
    /// <returns>如果实体条目是有效的审计条目，则返回 true；否则返回 false。</returns>
    private static bool IsValidAuditEntry(EntityEntry entry)
    {
        return entry.Entity is not AuditTrail
            && entry.State != EntityState.Detached
            && entry.State != EntityState.Unchanged;
    }

    /// <summary>
    /// 根据实体条目、当前用户 ID、当前日期时间和调试视图信息创建审计跟踪记录。
    /// 记录包括审计类型（创建、删除、更新）、受影响的列、新旧值等信息。
    /// </summary>
    /// <param name="entry">实体条目。</param>
    /// <param name="userId">当前用户 ID。</param>
    /// <param name="now">当前日期和时间。</param>
    /// <param name="debugView">实体条目的调试视图信息。</param>
    /// <returns>创建的审计跟踪记录实例。</returns>
    private AuditTrail CreateAuditTrail(
        EntityEntry entry,
        string userId,
        DateTime now,
        string? debugView
    )
    {
        var auditTrail = new AuditTrail
        {
            Id = Guid.CreateVersion7().ToString(),
            TableName = entry.Entity.GetType().Name,
            UserId = userId,
            DateTime = now,
            AffectedColumns = new List<string>(),
            NewValues = new Dictionary<string, object?>(),
            OldValues = new Dictionary<string, object?>(),
            DebugView = debugView
        };

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary)
            {
                auditTrail.TemporaryProperties.Add(property);
                continue;
            }

            var propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey() && property.CurrentValue != null)
            {
                auditTrail.PrimaryKey[propertyName] = property.CurrentValue;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditTrail.AuditType = AuditType.Create;
                    if (property.CurrentValue != null)
                        auditTrail.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditTrail.AuditType = AuditType.Delete;
                    if (property.OriginalValue != null)
                        auditTrail.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified
                    when property.IsModified
                        && !Equals(property.OriginalValue, property.CurrentValue):
                    auditTrail.AuditType = AuditType.Update;
                    auditTrail.AffectedColumns.Add(propertyName);
                    if (property.OriginalValue != null)
                        auditTrail.OldValues[propertyName] = property.OriginalValue;
                    if (property.CurrentValue != null)
                        auditTrail.NewValues[propertyName] = property.CurrentValue;
                    break;
            }
        }

        return auditTrail;
    }

    /// <summary>
    /// 完成审计跟踪记录的最终处理，包括处理临时属性并将审计跟踪记录保存到数据库中。
    /// </summary>
    /// <param name="context">数据库上下文实例。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    private async Task FinalizeAuditTrailsAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        if (_temporaryAuditTrailList.Count != 0)
        {
            foreach (var auditTrail in _temporaryAuditTrailList)
            {
                foreach (var prop in auditTrail.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue != null)
                    {
                        auditTrail.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditTrail.NewValues != null && prop.CurrentValue != null)
                    {
                        auditTrail.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }
            
            // 将临时审计跟踪记录添加到数据库上下文并保存
            await context.AddRangeAsync(_temporaryAuditTrailList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            _temporaryAuditTrailList.Clear();
        }
    }

    /// <summary>
    /// 使用新的数据库上下文保存审计跟踪记录，在保存更改失败时调用此方法。
    /// 包括处理临时属性并将审计跟踪记录保存到新的数据库上下文中。
    /// </summary>
    /// <param name="auditTrails">要保存的审计跟踪记录列表。</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    private async Task SaveAuditTrailsWithNewContextAsync(
        List<AuditTrail> auditTrails,
        CancellationToken cancellationToken
    )
    {
        if (_temporaryAuditTrailList.Count != 0)
        {
            foreach (var auditTrail in _temporaryAuditTrailList)
            {
                foreach (var prop in auditTrail.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue != null)
                    {
                        auditTrail.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditTrail.NewValues != null && prop.CurrentValue != null)
                    {
                        auditTrail.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }
            // 创建新的数据库上下文
            var dbcontext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            
            // 将审计跟踪记录添加到新的数据库上下文并保存
            await dbcontext.AddRangeAsync(auditTrails, cancellationToken);
            await dbcontext.SaveChangesAsync(cancellationToken);

            _temporaryAuditTrailList.Clear();
        }
    }
}
