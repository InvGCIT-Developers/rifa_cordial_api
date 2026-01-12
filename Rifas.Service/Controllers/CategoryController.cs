using GCIT.Core.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Interfaces;
using System.Threading.Tasks;

namespace Rifas.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearCategoryResponse), StatusCodes.Status201Created)]
        public Task<CrearCategoryResponse> CrearAsync([FromBody] CrearCategoryRequest request)
            => _service.CrearCategoriaAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarCategoryResponse), StatusCodes.Status200OK)]
        public Task<ActualizarCategoryResponse> ActualizarAsync([FromBody] ActualizarCategoryRequest request)
            => _service.ActualizarCategoriaAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => _service.EliminarCategoriaAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerCategoryPorIdResponse), StatusCodes.Status200OK)]
        public Task<ObtenerCategoryPorIdResponse> ObtenerPorIdAsync([FromRoute] long id)
            => Task.FromResult(new ObtenerCategoryPorIdResponse { EsExitoso = false, Mensaje = "Not implemented", Datos = null });

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarCategoryResponse), StatusCodes.Status200OK)]
        
        public Task<ListarCategoryResponse> ListarAsync([FromBody] ListarCategoryRequest request)
            => _service.ListarCategoriasAsync(request);
    }
}
