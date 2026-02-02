using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Client.Interfaces
{
    public interface ICategoryService
    {
        Task<CrearCategoryResponse> CrearCategoriaAsync(CrearCategoryRequest request);
        Task<ActualizarCategoryResponse> ActualizarCategoriaAsync(ActualizarCategoryRequest request);
        Task<BaseResponse> EliminarCategoriaAsync(long id);
        Task<ListarCategoryResponse> ListarCategoriasAsync(ListarCategoryRequest request);
        Task<ListarCategoryResponse> ListarCategoriasActivasAsync(ListarCategoryRequest request);
    }
}
