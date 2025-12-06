
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GCIT.Core.Models.Base;
using Rifas.Client.Services.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleService _service;

        public RaffleController(IRaffleService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearRaffleResponse), StatusCodes.Status201Created)]
        public Task<CrearRaffleResponse> CrearAsync([FromBody] CrearRaffleRequest request)
            => _service.CrearAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarRaffleResponse), StatusCodes.Status200OK)]
        public Task<ActualizarRaffleResponse> ActualizarAsync([FromBody] ActualizarRaffleRequest request)
            => _service.ActualizarAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => _service.EliminarAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerRafflePorIdResponse), StatusCodes.Status200OK)]
        public Task<ObtenerRafflePorIdResponse> ObtenerPorIdAsync([FromRoute] long id)
            => _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarRaffleResponse), StatusCodes.Status200OK)]
        public Task<ListarRaffleResponse> ListarAsync([FromBody] ListarRaffleRequest request)
            => _service.ListarAsync(request);
    }
}