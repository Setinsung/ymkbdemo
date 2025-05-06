using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

public class OnlineStatusInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private DotNetObjectReference<OnlineStatusInterop>? _dotNetRef;

    public event Action<bool>? OnlineStatusChanged;

    public void Initialize()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        jsRuntime.InvokeVoidAsync("onlineStatusInterop.addOnlineStatusListener", _dotNetRef);
    }

    public async Task<bool> GetOnlineStatusAsync()
    {
        return await jsRuntime.InvokeAsync<bool>("onlineStatusInterop.getOnlineStatus");
    }

    [JSInvokable]
    public void UpdateOnlineStatus(bool isOnline)
    {
        OnlineStatusChanged?.Invoke(isOnline);
    }

    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}

