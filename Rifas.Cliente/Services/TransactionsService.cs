
using System;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;

namespace Rifas.Client.Modulos.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _repository;

        public TransactionsService(ITransactionsRepository repository)
        {
            _repository = repository;
        }

        public Task<CrearTransactionsResponse> CrearAsync(CrearTransactionsRequest request)
            => throw new NotImplementedException();

       

        public Task<ObtenerTransactionsResponse> ObtenerPorIdAsync(long id)
            => throw new NotImplementedException();

        public Task<ListarTransactionsResponse> ListarAsync(ListarTransactionsRequest request)
            => throw new NotImplementedException();
    }
}