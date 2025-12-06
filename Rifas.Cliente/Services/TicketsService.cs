
using System;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;

namespace Rifas.Client.Modulos.Services
{
    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _repository;

        public TicketsService(ITicketsRepository repository)
        {
            _repository = repository;
        }

        public Task<CrearTicketsResponse> CrearAsync(CrearTicketsRequest request)
            => throw new NotImplementedException();

        public Task<ActualizarTicketsResponse> ActualizarAsync(ActualizarTicketsRequest request)
            => throw new NotImplementedException();

        public Task<BaseResponse> EliminarAsync(long id)
            => throw new NotImplementedException();

        public Task<ObtenerTicketsPorIdResponse> ObtenerPorIdAsync(long id)
            => throw new NotImplementedException();

        public Task<ListarTicketsResponse> ListarAsync(ListarTicketsRequest request)
            => throw new NotImplementedException();
    }
}