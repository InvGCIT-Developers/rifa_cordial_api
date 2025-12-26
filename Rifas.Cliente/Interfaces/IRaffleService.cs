
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Services.Interfaces
{
    public interface IRaffleService
    {
        Task<CrearRaffleResponse> CrearAsync(CrearRaffleRequest request);
        Task<ActualizarRaffleResponse> ActualizarAsync(ActualizarRaffleRequest request);
        Task<BaseResponse> EliminarAsync(long id);
        Task<ObtenerRafflePorIdResponse> ObtenerPorIdAsync(long id);
        Task<ListarRaffleResponse> ListarAsync(ListarRaffleRequest request);
        Task<VerificarRaffleResponse> ExisteRaffleAsync(VerificarRaffleNumberRequest request);
    }
}