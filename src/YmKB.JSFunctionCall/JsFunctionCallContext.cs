using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace YmKB.JSFunctionCall;
/// <summary>
/// 表示一个用于调用 JavaScript 函数的上下文类。
/// </summary>
public class JsFunctionCallContext : IDisposable
{
    /// <summary>
    /// 用于执行 JavaScript 代码的 V8 脚本引擎实例。
    /// </summary>
    public V8ScriptEngine Engine { get; } = new();

    public JsFunctionCallContext()
    {
        Engine.AddHostType("Console", typeof(Console));
        Engine.AddHostType("HttpClientHelper", typeof(HttpClientHelper));
    }
    
     /// <summary>
    /// 异步调用指定的 JavaScript 函数。
    /// </summary>
    /// <param name="script">包含要执行的 JavaScript 代码的脚本。</param>
    /// <param name="functionName">要调用的 JavaScript 函数的名称。</param>
    /// <param name="args">传递给 JavaScript 函数的参数。</param>
    /// <returns>一个表示异步操作的任务，该任务的结果是 JavaScript 函数的返回值。如果返回值是 Promise，则等待 Promise 解决并返回其结果；如果返回值是 undefined，则返回 null。</returns>
    public async Task<object?> FunctionCallAsync(string script, string functionName, params object[] args)
    {
        Engine.Execute(script);
        dynamic resultOrPromise = Engine.Invoke(functionName, args);
        var t = resultOrPromise?.GetType();
        if (t == Undefined.Value.GetType()) return null;
        bool isPromise = Engine.Script.Object.prototype.toString.call(resultOrPromise) == "[object Promise]";
        if (!isPromise) return resultOrPromise;
        // 获取Promise返回的结果
        var tcs = new TaskCompletionSource<object>();
        if(resultOrPromise is null) return null;
        resultOrPromise.then(
            new Action<object>(value =>
            {
                value = value.GetType().GetProperty("Result")?.GetValue(value) ?? value;
                tcs.SetResult(value);
            })
        );

        return await tcs.Task.ConfigureAwait(false);
    } 
    
    /// <summary>
    /// 异步调用指定的 JavaScript 函数，并将返回值转换为指定的类型。
    /// </summary>
    /// <typeparam name="T">期望的返回值类型。</typeparam>
    /// <param name="script">包含要执行的 JavaScript 代码的脚本。</param>
    /// <param name="functionName">要调用的 JavaScript 函数的名称。</param>
    /// <param name="args">传递给 JavaScript 函数的参数。</param>
    /// <returns>一个表示异步操作的任务，该任务的结果是 JavaScript 函数的返回值，并转换为指定的类型。如果返回值无法转换为指定类型，则返回该类型的默认值。</returns>
    public async Task<T?> FunctionCallAsync<T>(string script, string functionName, params object[] args)
    {
        var result = await FunctionCallAsync(script, functionName, args);
        if (result is T t) return t;
        return default;
    }
    
    /// <summary>
    /// 向脚本引擎中添加一个宿主对象。
    /// </summary>
    /// <param name="name">宿主对象的名称，用于在 JavaScript 代码中引用该对象。</param>
    /// <param name="obj">要添加的宿主对象实例。</param>
    public void AddHostObject(string name, object obj)
    {
        Engine.AddHostObject(name, obj);
    }
    
    /// <summary>
    /// 向脚本引擎中添加一个宿主类型。
    /// </summary>
    /// <param name="name">宿主类型的名称，用于在 JavaScript 代码中引用该类型。</param>
    /// <param name="type">要添加的宿主类型。</param>
    public void AddHostType(string name, Type type)
    {
        Engine.AddHostType(name, type);
    }

    
    public void Dispose()
    {
        Engine.Dispose();
        GC.SuppressFinalize(this);
    }
}