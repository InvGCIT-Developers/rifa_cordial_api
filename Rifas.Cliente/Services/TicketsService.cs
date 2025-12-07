using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
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
    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _repository;

        public TicketsService(ITicketsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CrearTicketsResponse> CrearAsync(CrearTicketsRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new CrearTicketsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_TICKET_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                entity.CreatedAt = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return new CrearTicketsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Ticket creado correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearTicketsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear el ticket: {ex.Message}",
                    CodigoError = "CREAR_TICKET_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ActualizarTicketsResponse> ActualizarAsync(ActualizarTicketsRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new ActualizarTicketsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "ACTUALIZAR_TICKET_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var existing = await _repository.GetByIdAsync(request.Datos.Id);
                if (existing == null)
                {
                    return new ActualizarTicketsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Ticket no encontrado",
                        CodigoError = "ACTUALIZAR_TICKET_NOT_FOUND",
                        Datos = null
                    };
                }

                var toUpdate = request.Datos.ToEntity();
                // preservar CreatedAt
                toUpdate.CreatedAt = existing.CreatedAt;

                await _repository.UpdateAsync(toUpdate);
                await _repository.SaveChangesAsync();

                return new ActualizarTicketsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Ticket actualizado correctamente",
                    Datos = toUpdate.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ActualizarTicketsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al actualizar el ticket: {ex.Message}",
                    CodigoError = "ACTUALIZAR_TICKET_ERROR",
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
                        Mensaje = "Ticket no encontrado",
                        CodigoError = "ELIMINAR_TICKET_NOT_FOUND"
                    };
                }

                await _repository.DeleteAsync(existing);
                await _repository.SaveChangesAsync();

                return new BaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Ticket eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al eliminar el ticket: {ex.Message}",
                    CodigoError = "ELIMINAR_TICKET_ERROR",
                    Errores = new[] { ex.Message }.ToList()
                };
            }
        }

        public async Task<ObtenerTicketsPorIdResponse> ObtenerPorIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ObtenerTicketsPorIdResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Ticket no encontrado",
                        CodigoError = "OBTENER_TICKET_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerTicketsPorIdResponse
                {
                    EsExitoso = true,
                    Mensaje = "Ticket obtenido correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerTicketsPorIdResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener el ticket: {ex.Message}",
                    CodigoError = "OBTENER_TICKET_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ListarTicketsResponse> ListarAsync(ListarTicketsRequest request)
        {
            try
            {
                var lista = await _repository
                        .AllNoTracking()
                        .Take(request.RegistrosPorPagina.Value)
                        .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                        .ToListAsync();

                return new ListarTicketsResponse
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
                return new ListarTicketsResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Mensaje = $"Error al listar los tickets: {ex.Message}",
                    CodigoError = "LISTAR_TICKETS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}