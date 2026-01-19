
using GCIT.Core.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;
using System.Threading.Tasks;

namespace Rifas.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _service;

        public TransactionsController(ITransactionsService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CrearTransactionsResponse), StatusCodes.Status201Created)]
        public Task<CrearTransactionsResponse> CrearAsync([FromBody] CrearTransactionsRequest request)
            => _service.CrearAsync(request);
       

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ObtenerTransactionsResponse), StatusCodes.Status200OK)]
        public Task<ObtenerTransactionsResponse> ObtenerPorIdAsync([FromRoute] long id)
            => _service.ObtenerPorIdAsync(id);

        [HttpPost("Listar")]
        [ProducesResponseType(typeof(ListarTransactionsResponse), StatusCodes.Status200OK)]
        public Task<ListarTransactionsResponse> ListarAsync([FromBody] ListarTransactionsRequest request)
            => _service.ListarAsync(request);
    }
}