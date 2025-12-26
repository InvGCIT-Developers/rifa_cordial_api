using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Interfaces
{
    public interface IPurchaseService
    {
        Task<CrearPurchaseResponse> CrearAsync(CrearPurchaseRequest request);
        Task<ActualizarPurchaseResponse> ActualizarAsync(ActualizarPurchaseRequest request);
        Task<BaseResponse> EliminarAsync(long id);
        Task<ObtenerPurchaseResponse> ObtenerPorIdAsync(long id);
        Task<ListarPurchaseResponse> ListarAsync(ListarPurchaseRequest request);
    }
}