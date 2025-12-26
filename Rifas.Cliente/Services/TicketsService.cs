using GCIT.Core.Models.Base;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Common;
using Rifas.Client.Mappers;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Rifas.Client.Modulos.Services
{
    public class TicketsService : ITicketsService
    {
        private readonly ITicketsRepository _repository;
        private readonly IRaffleRepository _rafflerepository;

        public TicketsService(ITicketsRepository repository, IRaffleRepository rafflerepository)
        {
            _repository = repository;
            _rafflerepository = rafflerepository;
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
                // join entre raffles y tickets usando los repositorios correspondientes
                var joined = _rafflerepository
                    .AllNoTracking()
                    .Join(
                        _repository.AllNoTracking(),
                        r => r.Id,
                        t => t.RaffleId,
                        (r, t) => new { Raffle = r, Ticket = t }
                    );


                // aplicar filtros opcionales si vienen en request.Filtros
                if (request?.Filtros != null && request.Filtros.Any())
                {
                    foreach (var filtro in request.Filtros)
                    {
                        if (filtro == null) continue;

                        // intentar obtener nombre del campo y valor soportando variantes (español/inglés)
                        var fType = filtro.GetType();
                        var campoProp = fType.GetProperty("Campo") ?? fType.GetProperty("Field") ?? fType.GetProperty("Name") ?? fType.GetProperty("Key");
                        var valorProp = fType.GetProperty("Valor") ?? fType.GetProperty("Value") ?? fType.GetProperty("Val");

                        var campo = campoProp?.GetValue(filtro)?.ToString();
                        var valor = valorProp?.GetValue(filtro)?.ToString();

                        if (string.IsNullOrWhiteSpace(campo) || valor == null) continue;

                        switch (campo.Trim().ToLowerInvariant())
                        {
                            case "raffleid":                            
                            case "raffle_id":
                            case "rafleid":
                                if (long.TryParse(valor, out var rId))
                                {
                                    joined = joined.Where(x => x.Raffle.Id == rId);
                                }
                                break;

                            case "ticketnumber":
                            case "ticket_number":
                            case "ticket":
                                if (long.TryParse(valor, out var tnum))
                                {
                                    joined = joined.Where(x => x.Ticket.TicketNumber == tnum);
                                }
                                break;

                            case "userid":
                            case "user_id":
                            case "user":
                                if (long.TryParse(valor, out var uId))
                                {
                                    joined = joined.Where(x => x.Ticket.UserId == uId);
                                }
                                break;

                            case "category":
                            case "categoria":
                                // category suele ser string o enum; comparamos como string (igualdad)
                                joined = joined.Where(x => x.Raffle.Category == (RifaCategoriaEnum)int.Parse(valor));
                                break;

                            case "status":
                            case "estado":
                                // intentar parsear como int/enum o comparar por descripción
                                if (int.TryParse(valor, out var statusInt))
                                {
                                    joined = joined.Where(x => (int)x.Ticket.Status == statusInt);
                                }
                                else
                                {
                                    joined = joined.Where(x => x.Ticket.StatusDescription.Contains(valor));
                                }
                                break;

                            case "rafflename":
                            case "title":
                            case "nombre":
                            case "raffle_name":
                                joined = joined.Where(x => x.Raffle.Title.Contains(valor));
                                break;

                            case "purchasedatfrom":
                            case "purchased_from":
                            case "buyeddatefrom":
                            case "comprado_desde":
                            case "purchasedatdesde":
                                if (DateTime.TryParse(valor, out var fromDt))
                                {
                                    joined = joined.Where(x => x.Ticket.BuyedDate != null && x.Ticket.BuyedDate >= fromDt);
                                }
                                break;

                            case "purchasedatto":
                            case "purchased_to":
                            case "buyeddateto":
                            case "comprado_hasta":
                            case "purchasedathasta":
                                if (DateTime.TryParse(valor, out var toDt))
                                {
                                    joined = joined.Where(x => x.Ticket.BuyedDate != null && x.Ticket.BuyedDate <= toDt);
                                }
                                break;

                            default:
                                // si el campo no es conocido, se ignora; se pueden añadir más casos según necesidad
                                break;
                        }
                    }
                }

                // buscar texto global en todos los campos relevantes si viene request.Buscar
                if (!string.IsNullOrWhiteSpace(request?.Buscar))
                {
                    var term = request.Buscar.Trim();
                    var isLong = long.TryParse(term, out var termLong);
                    var isInt = int.TryParse(term, out var termInt);
                    var isDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out var termDecimal);
                    var isDate = DateTime.TryParse(term, out var termDate);

                    joined = joined.Where(x =>
                        // campos string
                        (x.Raffle.Title != null && EF.Functions.Like(x.Raffle.Title, $"%{term}%")) ||
                        (x.Raffle.Description != null && EF.Functions.Like(x.Raffle.Description, $"%{term}%")) ||
                        (x.Raffle.ImageUrl != null && EF.Functions.Like(x.Raffle.ImageUrl, $"%{term}%")) ||
                        (x.Raffle.Organizer != null && EF.Functions.Like(x.Raffle.Organizer, $"%{term}%")) ||
                        (x.Ticket.StatusDescription != null && EF.Functions.Like(x.Ticket.StatusDescription, $"%{term}%")) ||

                        // búsqueda por enum nombre (si no es numérico)
                        (!isInt && x.Raffle.Category.ToString().Contains(term)) ||

                        // comparaciones numéricas cuando el término es numérico
                        (isLong && (x.Raffle.Id == termLong || x.Ticket.TicketNumber == termLong || x.Ticket.UserId == termLong)) ||

                        // precio comparado si decimal
                        (isDecimal && x.Raffle.Price != null && x.Raffle.Price == termDecimal) ||

                        // búsquedas por fecha aproximada (día)
                        (isDate && x.Ticket.BuyedDate != null && x.Ticket.BuyedDate >= termDate && x.Ticket.BuyedDate < termDate.AddDays(1))
                    );
                }

                // contar elementos totales antes de paginar
                var totalElementos = await joined.CountAsync();

                // aplicar ordenación por Id de ticket descendente, paginación y proyección a DTO de listado
                var lista = await joined
                    .OrderByDescending(x => x.Ticket.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .Select(x => new TicketsListadoDTO
                    {
                        RaffleId = x.Raffle.Id,
                        RaffleName = x.Raffle.Title,
                        RaffleImage = x.Raffle.ImageUrl,
                        TicketNumber = x.Ticket.TicketNumber,
                        Note = x.Ticket.StatusDescription,
                        Category = x.Raffle.Category,
                        Status = x.Ticket.Status,
                        PurchasedAt = x.Ticket.BuyedDate
                    })
                    .ToListAsync();

                return new ListarTicketsResponse
                {
                    EsExitoso = true,
                    TotalElementos = totalElementos,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    TotalPaginas = (int)Math.Ceiling((double)totalElementos / request.RegistrosPorPagina.Value),
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Datos = lista
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