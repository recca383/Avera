using Avera.WebApi.Extensions;
using Avera.Application;
using Avera.Infrastructure;
using Scalar.AspNetCore;
using System.Reflection;
using Azure.Identity;
using Avera.WebApi.Infrastructure;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Uri"]!),
    new DefaultAzureCredential()
);
builder.Services.AddHttpClient();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseAntiforgery();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();

app.Run();

