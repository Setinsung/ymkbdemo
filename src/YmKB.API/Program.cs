using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using YmKB.API.Common;
using YmKB.API.Endpoints;
using YmKB.API.ExceptionHandlers;
using YmKB.Application;
using YmKB.Application.Contracts.Identity;
using YmKB.Infrastructure;
using YmKB.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterSerilog();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder
    .Services
    .AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance =
                $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            context
                .ProblemDetails
                .Extensions
                .TryAdd("requestId", context.HttpContext.TraceIdentifier);
            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            if (activity != null)
            {
                context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id);
            }
        };
    });
builder
    .Services
    .ConfigureHttpJsonOptions(options =>
    {
        // Don't serialize null values
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // options.SerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
        // Pretty print JSON
        options.SerializerOptions.WriteIndented = true;
    });

builder.Services.AddExceptionHandler<ProblemExceptionHandler>();

builder.Services.AddControllers();
// builder.Services.AddControllers(options =>
// {
//     options.ModelBinderProviders.Insert(0, new NullableEnumModelBinderProvider());
// });

builder
    .Services
    .AddCors(options =>
    {
        options.AddPolicy("all", cfg => cfg.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.AssignableTo<IEndpointRegistrar>())
    .As<IEndpointRegistrar>()
    .WithScopedLifetime());

builder.Services.AddOpenApi(opt =>
{
    opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

});
builder.Services.AddAntiforgery();


var app = builder.Build();
await app.InitializeDatabaseAsync();
app.UseExceptionHandler();

// 用于设置当前用户的上下文
app.Use(
    async (context, next) =>
    {
        var currentUserContextSetter = context
            .RequestServices
            .GetRequiredService<ICurrentUserContextSetter>();
        try
        {
            currentUserContextSetter.SetCurrentUser(context.User);
            await next.Invoke();
        }
        finally
        {
            currentUserContextSetter.Clear();
        }
    }
);
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"files")))
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"files"));
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"files")
        ),
        RequestPath = new PathString("/files")
    }
);
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();
app.UseCors("all");
app.UseAntiforgery();

app.UseAuthorization();

app.MapEndpointDefinitions();
app.MapControllers();

app.Run();
