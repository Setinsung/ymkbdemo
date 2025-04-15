using FluentValidation;
using Mediator;
using YmKB.Application.Extensions;

namespace YmKB.Application.Pipeline;
/// <summary>
/// 消息验证器行为类，用于在消息处理前对消息进行验证。
/// </summary>
/// <typeparam name="TMessage">要处理的消息类型，需实现 IMessage 和 IRequiresValidation 接口。</typeparam>
/// <typeparam name="TResponse">消息处理后的响应类型。</typeparam>
public sealed class MessageValidatorBehaviour<TMessage, TResponse>
    : MessagePreProcessor<TMessage, TResponse>
    where TMessage : IMessage, IRequiresValidation
{
    private readonly IReadOnlyCollection<IValidator<TMessage>> _validators;

    public MessageValidatorBehaviour(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));
    }

    /// <summary>
    /// 在消息处理前进行验证。
    /// </summary>
    /// <param name="message">要处理的消息。</param>
    /// <param name="cancellationToken">用于取消异步操作的取消令牌。</param>
    /// <returns>表示异步操作的 ValueTask。</returns>
    /// <exception cref="ValidationException">当消息验证不通过时抛出此异常。</exception>
    protected override async ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TMessage>(message);
        var validationResult = await _validators.ValidateAsync(context, cancellationToken);
        if (validationResult.Count != 0)
        {
            throw new ValidationException(validationResult);
        }
    }
}