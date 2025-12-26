using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Interfaces;

namespace Rifas.Client.Modulos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchaseController(IPurchaseService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearPurchaseResponse), StatusCodes.Status201Created)]
        public Task<CrearPurchaseResponse> CrearAsync([FromBody] CrearPurchaseRequest request)
            => _service.CrearAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarPurchaseResponse), StatusCodes.Status200OK)]
        public Task<ActualizarPurchaseResponse> ActualizarAsync([FromBody] ActualizarPurchaseRequest request)
            => _service.ActualizarAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => _service.EliminarAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerPurchaseResponse), StatusCodes.Status200OK)]
        public Task<ObtenerPurchaseResponse> ObtenerPorIdAsync([FromRoute] long id)
            => _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarPurchaseResponse), StatusCodes.Status200OK)]
        public Task<ListarPurchaseResponse> ListarAsync([FromBody] ListarPurchaseRequest request)
            => _service.ListarAsync(request);
    }
}