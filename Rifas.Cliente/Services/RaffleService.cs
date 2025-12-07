using System;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Services.Interfaces;
using Rifas.Client.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Mappers;

namespace Rifas.Client.Modulos.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly IRaffleRepository _repository;

        public RaffleService(IRaffleRepository repository)
        {
            _repository = repository;
        }

        public async Task<CrearRaffleResponse> CrearAsync(CrearRaffleRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new CrearRaffleResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_RAFFLE_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                entity.CreatedAt = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return new CrearRaffleResponse
                {
                    EsExitoso = true,
                    Mensaje = "Rifa creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearRaffleResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear la rifa: {ex.Message}",
                    CodigoError = "CREAR_RAFFLE_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ActualizarRaffleResponse> ActualizarAsync(ActualizarRaffleRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new ActualizarRaffleResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "ACTUALIZAR_RAFFLE_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var existing = await _repository.GetByIdAsync(request.Datos.Id);
                if (existing == null)
                {
                    return new ActualizarRaffleResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Rifa no encontrada",
                        CodigoError = "ACTUALIZAR_RAFFLE_NOT_FOUND",
                        Datos = null
                    };
                }

                var toUpdate = request.Datos.ToEntity();
                // preservar CreatedAt del registro existente
                toUpdate.CreatedAt = existing.CreatedAt;

                await _repository.UpdateAsync(toUpdate);
                await _repository.SaveChangesAsync();

                return new ActualizarRaffleResponse
                {
                    EsExitoso = true,
                    Mensaje = "Rifa actualizada correctamente",
                    Datos = toUpdate.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ActualizarRaffleResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al actualizar la rifa: {ex.Message}",
                    CodigoError = "ACTUALIZAR_RAFFLE_ERROR",
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
                        Mensaje = "Rifa no encontrada",
                        CodigoError = "ELIMINAR_RAFFLE_NOT_FOUND"
                    };
                }

                await _repository.DeleteAsync(existing);
                await _repository.SaveChangesAsync();

                return new BaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Rifa eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al eliminar la rifa: {ex.Message}",
                    CodigoError = "ELIMINAR_RAFFLE_ERROR",
                    Errores = new[] { ex.Message }.ToList()
                };
            }
        }

        public async Task<ObtenerRafflePorIdResponse> ObtenerPorIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ObtenerRafflePorIdResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Rifa no encontrada",
                        CodigoError = "OBTENER_RAFFLE_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerRafflePorIdResponse
                {
                    EsExitoso = true,
                    Mensaje = "Rifa obtenida correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerRafflePorIdResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener la rifa: {ex.Message}",
                    CodigoError = "OBTENER_RAFFLE_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ListarRaffleResponse> ListarAsync(ListarRaffleRequest request)
        {
            try
            {
                var lista = await _repository
                        .AllNoTracking()
                        .Take(request.RegistrosPorPagina.Value)
                        .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                        .ToListAsync();

                return new ListarRaffleResponse
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

                return new ListarRaffleResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                     Mensaje = $"Error al listar los rifas: {ex.Message}",
                     CodigoError = "LISTAR_RAFFLES_ERROR",
                      Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}