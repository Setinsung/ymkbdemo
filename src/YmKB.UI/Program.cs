using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Extensions;
using YmKB.UI;
using YmKB.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServicesWithExtensions();
builder.Services.AddBlazoredLocalStorage();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder
    .Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7219") });
builder.Services.AddScoped<IKnowledgeBaseService, MockKnowledgeBaseService>();
builder.Services.AddScoped<IDocumentService, MockDocumentService>();
builder.Services.AddScoped<IApplicationService, MockApplicationService>();
builder.Services.AddScoped<IAIModelService, MockAIModelService>();

await builder.Build().RunAsync();
