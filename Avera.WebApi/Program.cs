using Avera.WebApi.Extensions;
using Avera.Application;
using Avera.Infrastructure;
using Scalar.AspNetCore;
using System.Reflection;
using Azure.Identity;
using Avera.WebApi.Infrastructure;
using dotenv.net;
using Serilog;
using Avera.WebApi;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Uri"]!),
    new DefaultAzureCredential()
);

builder.Services
    .AddWebApi()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<ApiKeyMiddleware>();

app.MapEndpoints();

app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);

        var key = app.Configuration["EXTERNAL-API-KEY"];

        options.AddPreferredSecuritySchemes("Bearer");

    });
}

app.Lifetime.ApplicationStopping.Register(() => Log.CloseAndFlush());

app.Run();

