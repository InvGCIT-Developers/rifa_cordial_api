
using System;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;

namespace Rifas.Client.Modulos.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly IRaffleRepository _repository;

        public RaffleService(IRaffleRepository repository)
        {
            _repository = repository;
        }

        public Task<CrearRaffleResponse> CrearAsync(CrearRaffleRequest request)
            => throw new NotImplementedException();

        public Task<ActualizarRaffleResponse> ActualizarAsync(ActualizarRaffleRequest request)
            => throw new NotImplementedException();

        public Task<BaseResponse> EliminarAsync(long id)
            => throw new NotImplementedException();

        public Task<ObtenerRafflePorIdResponse> ObtenerPorIdAsync(long id)
            => throw new NotImplementedException();

        public Task<ListarRaffleResponse> ListarAsync(ListarRaffleRequest request)
            => throw new NotImplementedException();
    }
}