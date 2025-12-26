using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Rifas.Client;
using Rifas.Client.Data;
using GCIT.Core;

namespace Rifas.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Registrar DbContext y repositorios desde Rifas.Cliente
                    var configuration = context.Configuration;

                    // Registrar DbContext con la cadena DefaultDB (igual que en la API).
                    // Esto usa la extensión existente en el proyecto principal:
                    services.AddInfrastructureWithContext<RifasContext>(configuration,
                        options => options.UseSqlServer(
                            configuration.GetConnectionString("DefaultDB"),
                            sql => sql.EnableRetryOnFailure()
                        ));

                    // Registrar servicios/repositories definidos en Rifas.Cliente (DependencyInjection.AddInfrastructure)
                    services.AddInfrastructure(configuration);

                    // Registrar el worker
                    services.AddHostedService<Services.ResultsGeneratorWorker>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();
                })
                .Build();

            host.Run();
        }
    }
}