
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Services.Interfaces
{
    public interface ITransactionsService
    {
        Task<CrearTransactionsResponse> CrearAsync(CrearTransactionsRequest request);    
        
        Task<ObtenerTransactionsResponse> ObtenerPorIdAsync(long id);
        Task<ListarTransactionsResponse> ListarAsync(ListarTransactionsRequest request);
    }
}