
using GCIT.Core.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;
using System.Threading.Tasks;

namespace Rifas.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
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
        [AllowAnonymous]
        public Task<ListarTicketsResponse> ListarAsync([FromBody] ListarTicketsRequest request)
            => _service.ListarAsync(request);
    }
}