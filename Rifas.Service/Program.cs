using GCIT.Core;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddInfrastructureWithContext<RifasContext>(builder.Configuration,
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultDB"),
        sql =>
        {
            //sql.MigrationsAssembly(typeof(LiveWalletContext).Assembly.FullName);
            sql.EnableRetryOnFailure();
        }));

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<RifasContext>());
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
