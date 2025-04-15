namespace YmKB.Domain.Abstractions.Common;

/// <summary>
/// 实体接口，所有实体都应该实现这个接口
/// </summary>
public interface IEntity;

/// <summary>
/// 自定义主键类型 的实体接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEntity<T> : IEntity
{
    /// <summary>
    /// 主键
    /// </summary>
    T Id { get; set; }
}
