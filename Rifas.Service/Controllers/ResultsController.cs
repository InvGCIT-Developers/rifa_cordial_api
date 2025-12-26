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
    public class ResultsController : ControllerBase
    {
        private readonly IResultsService _service;

        public ResultsController(IResultsService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearResultsResponse), StatusCodes.Status201Created)]
        public async Task<CrearResultsResponse> CrearAsync([FromBody] CrearResultsRequest request)
            => await _service.CrearAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarResultsResponse), StatusCodes.Status200OK)]
        public async Task<ActualizarResultsResponse> ActualizarAsync([FromBody] ActualizarResultsRequest request)
            => await _service.ActualizarAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public async Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => await _service.EliminarAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerResultsResponse), StatusCodes.Status200OK)]
        public async Task<ObtenerResultsResponse> ObtenerPorIdAsync([FromRoute] long id)
            => await _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarResultsResponse), StatusCodes.Status200OK)]
        public async Task<ListarResultsResponse> ListarAsync([FromBody] ListarResultsRequest request)
            => await _service.ListarAsync(request);
    }
}