using System;
using System.Linq;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Interfaces;
using Rifas.Client.Mappers;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Repositories.Interfaces;

namespace Rifas.Client.Modulos.Services
{
    public class ResultsService : IResultsService
    {
        private readonly IResultsRepository _repository;

        public ResultsService(IResultsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CrearResultsResponse> CrearAsync(CrearResultsRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new CrearResultsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_RESULTS_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                entity.CreatedAt = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return new CrearResultsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Result creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearResultsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear el result: {ex.Message}",
                    CodigoError = "CREAR_RESULTS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ActualizarResultsResponse> ActualizarAsync(ActualizarResultsRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new ActualizarResultsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "ACTUALIZAR_RESULTS_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var existing = await _repository.GetByIdAsync(request.Datos.Id);
                if (existing == null)
                {
                    return new ActualizarResultsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Result no encontrado",
                        CodigoError = "ACTUALIZAR_RESULTS_NOT_FOUND",
                        Datos = null
                    };
                }

                var toUpdate = request.Datos.ToEntity();
                toUpdate.CreatedAt = existing.CreatedAt;

                await _repository.UpdateAsync(toUpdate);
                await _repository.SaveChangesAsync();

                return new ActualizarResultsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Result actualizado correctamente",
                    Datos = toUpdate.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ActualizarResultsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al actualizar el result: {ex.Message}",
                    CodigoError = "ACTUALIZAR_RESULTS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<BaseResponse> EliminarAsync(long id)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    return new BaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Result no encontrado",
                        CodigoError = "ELIMINAR_RESULTS_NOT_FOUND"
                    };
                }

                await _repository.DeleteAsync(existing);
                await _repository.SaveChangesAsync();

                return new BaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Result eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al eliminar el result: {ex.Message}",
                    CodigoError = "ELIMINAR_RESULTS_ERROR",
                    Errores = new[] { ex.Message }.ToList()
                };
            }
        }

        public async Task<ObtenerResultsResponse> ObtenerPorIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ObtenerResultsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Result no encontrado",
                        CodigoError = "OBTENER_RESULTS_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerResultsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Result obtenido correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerResultsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener el result: {ex.Message}",
                    CodigoError = "OBTENER_RESULTS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ListarResultsResponse> ListarAsync(ListarResultsRequest request)
        {
            try
            {
                var lista = await _repository
                    .AllNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarResultsResponse
                {
                    EsExitoso = true,
                    TotalElementos = lista.Count,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    TotalPaginas = (int)Math.Ceiling((double)lista.Count / request.RegistrosPorPagina.Value),
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Datos = lista.ToDtoList()
                };
            }
            catch (Exception ex)
            {
                return new ListarResultsResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Mensaje = $"Error al listar los results: {ex.Message}",
                    CodigoError = "LISTAR_RESULTS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}