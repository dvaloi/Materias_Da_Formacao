using ProvaVida.Infrastructure;
using ProvaVida.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 3º microsserviço — Prova de Vida (banco inss_prova_vida). Porta local :5003.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
    builder.WebHost.UseUrls($"http://+:{port}");

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

// TODO: publicar APIs na nuvem (Render/Railway) e apontar ContribuintesApi:BaseUrl via variável de ambiente.
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileDev", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=inss_prova_vida;Username=inss;Password=inss123";

var contribuintesApiUrl = builder.Configuration["ContribuintesApi:BaseUrl"]
    ?? "http://localhost:5001";

builder.Services.AddInfrastructure(connectionString, contribuintesApiUrl);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProvaVidaDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("MobileDev");
app.MapControllers();

app.Run();

public partial class Program { }
