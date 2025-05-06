using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

public sealed class DownloadFileInterop(IJSRuntime jsRuntime)
{
    public async Task DownloadFileFromStream(string fileName, DotNetStreamReference stream)
    {
        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, stream);
    }
}
