using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

public class ClipboardInterop(IJSRuntime jsRuntime)
{
    public async Task CopyToClipboard(string text)
    {
        await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}