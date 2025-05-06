using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

public sealed class DisplayModeInterop(IJSRuntime jsRuntime)
{
    public async Task<string> GetDisplayModeAsync()
    {
        return await jsRuntime.InvokeAsync<string>("displayModeInterop.getDisplayMode");
    }
}
