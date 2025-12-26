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
        public Task<CrearResultsResponse> CrearAsync([FromBody] CrearResultsRequest request)
            => _service.CrearAsync(request);

        [HttpPut]
        [ProducesResponseType(typeof(ActualizarResultsResponse), StatusCodes.Status200OK)]
        public Task<ActualizarResultsResponse> ActualizarAsync([FromBody] ActualizarResultsRequest request)
            => _service.ActualizarAsync(request);

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public Task<BaseResponse> EliminarAsync([FromRoute] long id)
            => _service.EliminarAsync(id);

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerResultsResponse), StatusCodes.Status200OK)]
        public Task<ObtenerResultsResponse> ObtenerPorIdAsync([FromRoute] long id)
            => _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarResultsResponse), StatusCodes.Status200OK)]
        public Task<ListarResultsResponse> ListarAsync([FromBody] ListarResultsRequest request)
            => _service.ListarAsync(request);
    }
}