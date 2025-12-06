using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rifas.Client.Modulos.Services;
using Rifas.Client.Repositories;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Services.Interfaces;
namespace Rifas.Client
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services here
            services.AddHttpContextAccessor();

            services.AddScoped<IRaffleService, RaffleService>();
            services.AddScoped<ITicketsService, TicketsService>();
            services.AddScoped<ITransactionsService, TransactionsService>();

            services.AddScoped<IRaffleRepository, RaffleRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();
            services.AddScoped<ITransactionsRepository, TransactionsRepository>();

            return services;

        }
    }
}
