
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Services.Interfaces
{
    public interface ITicketsService
    {
        Task<CrearTicketsResponse> CrearAsync(CrearTicketsRequest request);
        Task<ActualizarTicketsResponse> ActualizarAsync(ActualizarTicketsRequest request);
        Task<BaseResponse> EliminarAsync(long id);
        Task<ObtenerTicketsPorIdResponse> ObtenerPorIdAsync(long id);
        Task<ListarTicketsResponse> ListarAsync(ListarTicketsRequest request);
    }
}