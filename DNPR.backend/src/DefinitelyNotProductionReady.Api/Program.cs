using System.Text.Json.Serialization;
using DefinitelyNotProductionReady.Api.Middleware;
using DefinitelyNotProductionReady.Application.PostApplication;
using DefinitelyNotProductionReady.Application.Security;
using DefinitelyNotProductionReady.Application.UserApplication;
using DefinitelyNotProductionReady.Infrastructure.Persistence;
using DefinitelyNotProductionReady.Infrastructure.Security;
using PostApp = DefinitelyNotProductionReady.Application.PostApplication.PostApplication;
using UserApp = DefinitelyNotProductionReady.Application.UserApplication.UserApplication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

// Composition root: API wires Application use cases to Infrastructure implementations.
// Application does not reference Infrastructure.
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();
builder.Services.AddScoped<UserApp>();
builder.Services.AddScoped<PostApp>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();

app.MapControllers();
app.Run();

public partial class Program;
