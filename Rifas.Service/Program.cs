using GCIT.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Rifas.Client;
using Rifas.Client.Data;
using Rifas.Client.Modulos.Services;
using Rifas.Client.Services.Interfaces;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// En lugar de llamar siempre a UseUrls, hazlo s�lo en contenedor
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
builder.Services.AddInfrastructure(builder.Configuration);
// CORS policy llamada "acceder"
builder.Services.AddCors(options =>
{
    options.AddPolicy("acceder", policy =>
    {
        // Ajusta or�genes seg�n tu front-end; por defecto permite cualquier origen.
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


var openApiSecuritySchemeReference = new OpenApiSecuritySchemeReference(
    "Bearer", // referenceId: el id del esquema de seguridad
    null,     // OpenApiDocument: puedes pasar null si no tienes el documento
    "JWT Bearer token" // description: una descripción opcional
)
{
    Reference = new OpenApiReferenceWithDescription
    {
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer",
        Description = "JWT Bearer token"
    }
};

var openSecurityReq = new OpenApiSecurityRequirement();

openSecurityReq.Add(openApiSecuritySchemeReference, new List<string>());

builder.Services.AddEndpointsApiExplorer();
// Swashbuckle: genera documentaci�n OpenAPI y UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rifas API",
        Version = "v1",
        Description = "API para la gesti�n de rifas, tickets y transacciones."
    });
    // JWT in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization header usando el esquema Bearer. Ej: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });    
    //c.AddSecurityRequirement(openSecurityReq);       
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
    throw new InvalidOperationException("Jwt:Key no est� configurado en appsettings");
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
        // c.RoutePrefix = string.Empty; // descomenta para servir UI en la ra�z (/)
    });

    // Si prefieres la extensi�n existente:
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Usar CORS "acceder"
app.UseCors("acceder");

// Autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
