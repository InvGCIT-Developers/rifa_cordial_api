using GCIT.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Rifas.Client.Data;
using Rifas.Client.Modulos.Services;
using Rifas.Client.Services.Interfaces;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// En lugar de llamar siempre a UseUrls, hazlo sólo en contenedor
var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (runningInContainer)
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080", "https://0.0.0.0:8081");
}

// Add services to the container.
builder.Services.AddInfrastructureWithContext<RifasContext>(builder.Configuration,
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultDB"),
        sql =>
        {
            sql.EnableRetryOnFailure();
        }));

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<RifasContext>());

// CORS policy llamada "acceder"
builder.Services.AddCors(options =>
{
    options.AddPolicy("acceder", policy =>
    {
        // Ajusta orígenes según tu front-end; por defecto permite cualquier origen.
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

//var securityReq = new OpenApiSecurityRequirement()
//{
//    {
//        new OpenApiSecurityScheme
//        {
            
//            Reference = new OpenApiReference
//            {
//                Type = ReferenceType.SecurityScheme,
//                Id = "Bearer"
//            }
//        },
//        new List<string>()
//    }
//};

builder.Services.AddEndpointsApiExplorer();
// Swashbuckle: genera documentación OpenAPI y UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rifas API",
        Version = "v1",
        Description = "API para la gestión de rifas, tickets y transacciones."
    });
    // JWT in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization header usando el esquema Bearer. Ej: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    //c.AddSecurityRequirement(securityReq);
});

// (Puedes mantener AddOpenApi si necesitas, no es obligatorio)
builder.Services.AddOpenApi();

// Registrar AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Configurar JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key no está configurado en appsettings");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();


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

// Usar CORS "acceder"
app.UseCors("acceder");

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
