using Microsoft.JSInterop;

namespace YmKB.UI.Services.JsInterops;

public class FancyboxInterop(IJSRuntime jsRuntime)
{
    public async Task<ValueTask> Preview(string defaultUrl, IEnumerable<string> images)
    {
        var jsmodule = await jsRuntime
            .InvokeAsync<IJSObjectReference>("import", "/js/fancybox.js")
            .ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync("filepreview", defaultUrl, images);
    }
}
