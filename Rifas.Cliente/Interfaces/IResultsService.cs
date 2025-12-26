using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Interfaces
{
    public interface IResultsService
    {
        Task<CrearResultsResponse> CrearAsync(CrearResultsRequest request);
        Task<ActualizarResultsResponse> ActualizarAsync(ActualizarResultsRequest request);
        Task<BaseResponse> EliminarAsync(long id);
        Task<ObtenerResultsResponse> ObtenerPorIdAsync(long id);
        Task<ListarResultsResponse> ListarAsync(ListarResultsRequest request);
    }
}