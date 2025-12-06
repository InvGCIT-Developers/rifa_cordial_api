using GCIT.Core;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Data;

var builder = WebApplication.CreateBuilder(args);

// Forzar que Kestrel escuche en 0.0.0.0:8080/8081 (útil en contenedor)
builder.WebHost.UseUrls("http://0.0.0.0:8080", "https://0.0.0.0:8081");

// Add services to the container.
builder.Services.AddInfrastructureWithContext<RifasContext>(builder.Configuration,
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultDB"),
        sql =>
        {
            sql.EnableRetryOnFailure();
        }));

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<RifasContext>());

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
// Swashbuckle: genera documentación OpenAPI y UI
builder.Services.AddSwaggerGen();

// (Puedes mantener AddOpenApi si necesitas, no es obligatorio)
builder.Services.AddOpenApi();

var app = builder.Build();

// habilitar Swagger cuando estemos en Development o dentro de un contenedor
var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (app.Environment.IsDevelopment() || runningInContainer)
{
    // Web API OpenAPI endpoints + Swagger UI
    app.UseSwagger(); // expone /swagger/v1/swagger.json
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rifas API V1");
        // c.RoutePrefix = string.Empty; // descomenta para servir UI en la raíz (/)
    });

    // Si prefieres la extensión existente:
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
