using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace YmKB.Infrastructure.Persistence.Extensions;

public static class EntityEntryExtensions
{
    /// <summary>
    /// 此扩展方法用于判断实体条目中是否存在已更改的从属实体。
    /// 从属实体是指在实体框架中，与另一个实体存在紧密关联且其生命周期依赖于该实体的实体。
    /// 此方法会检查实体的所有引用，查看是否存在从属实体且其状态为已添加或已修改。
    /// </summary>
    /// <param name="entry">要检查的实体条目。</param>
    /// <returns>如果存在已更改（添加或修改）的从属实体，返回 true；否则返回 false。</returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry
            .References
            .Any(
                r =>
                    r.TargetEntry != null
                    && r.TargetEntry.Metadata.IsOwned()
                    && r.TargetEntry.State is EntityState.Added or EntityState.Modified
            );
    }
}
