using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Extensions;
using YmKB.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServicesWithExtensions();
builder.Services.AddBlazoredLocalStorage();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7219") });

await builder.Build().RunAsync();
