
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GCIT.Core.Models.Base;
using Rifas.Client.Services.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;

namespace Rifas.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _service;

        public TicketsController(ITicketsService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearTicketsResponse), StatusCodes.Status201Created)]
        public Task<CrearTicketsResponse> CrearAsync([FromBody] CrearTicketsRequest request)
            => _service.CrearAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarTicketsResponse), StatusCodes.Status200OK)]
        public Task<ActualizarTicketsResponse> ActualizarAsync([FromBody] ActualizarTicketsRequest request)
            => _service.ActualizarAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => _service.EliminarAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerTicketsPorIdResponse), StatusCodes.Status200OK)]
        public Task<ObtenerTicketsPorIdResponse> ObtenerPorIdAsync([FromRoute] long id)
            => _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarTicketsResponse), StatusCodes.Status200OK)]
        public Task<ListarTicketsResponse> ListarAsync([FromBody] ListarTicketsRequest request)
            => _service.ListarAsync(request);
    }
}