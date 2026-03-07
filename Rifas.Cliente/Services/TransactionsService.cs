using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
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
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepository _repository;

        public TransactionsService(ITransactionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CrearTransactionsResponse> CrearAsync(CrearTransactionsRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    request?.Datos.Id = null;
                    return new CrearTransactionsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_TRANSACTION_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                entity.CreatedAt = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return new CrearTransactionsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Transacciùn creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearTransactionsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear la transacciùn: {ex.Message}",
                    CodigoError = "CREAR_TRANSACTION_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ObtenerTransactionsResponse> ObtenerPorIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ObtenerTransactionsResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Transacciùn no encontrada",
                        CodigoError = "OBTENER_TRANSACTION_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerTransactionsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Transacciùn obtenida correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerTransactionsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener la transacciùn: {ex.Message}",
                    CodigoError = "OBTENER_TRANSACTION_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ListarTransactionsResponse> ListarAsync(ListarTransactionsRequest request)
        {
            try
            {
                var query = _repository.AllNoTracking();

                // aplicar filtros opcionales si vienen en request.Filtros
                if (request?.Filtros != null && request.Filtros.Any())
                {
                    foreach (var filtro in request.Filtros)
                    {
                        if (filtro == null) continue;

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
                                if (long.TryParse(valor, out var rId))
                                    query = query.Where(x => x.RaffleId == rId);
                                break;

                            case "ticketnumber":
                            case "ticket_number":
                                if (long.TryParse(valor, out var tnum))
                                    query = query.Where(x => x.TicketNumber == tnum);
                                break;

                            case "userid":
                            case "user_id":
                                if (long.TryParse(valor, out var uId))
                                    query = query.Where(x => x.UserId == uId);
                                break;

                            case "agenteid":
                            case "agente_id":
                                if (long.TryParse(valor, out var aId))
                                    query = query.Where(x => x.AgenteId == aId);
                                break;

                            case "action":
                            case "accion":
                                query = query.Where(x => x.Action != null && x.Action.Contains(valor));
                                break;

                            case "user":
                            case "username":
                            case "usuario":
                                query = query.Where(x => x.User != null && x.User.Contains(valor));
                                break;

                            case "agente":
                                query = query.Where(x => x.Agente != null && x.Agente.Contains(valor));
                                break;

                            case "ip":
                                query = query.Where(x => x.IP != null && x.IP.Contains(valor));
                                break;

                            case "datefrom":
                            case "date_from":
                            case "fechadesde":
                            case "datedesde":
                                if (DateTime.TryParse(valor, out var fromDt))
                                    query = query.Where(x => x.Date.Date >= fromDt.Date);
                                break;

                            case "dateto":
                            case "date_to":
                            case "fechahasta":
                            case "datehasta":
                                if (DateTime.TryParse(valor, out var toDt))
                                    query = query.Where(x => x.Date.Date <= toDt.Date);
                                break;

                            case "createdatfrom":
                            case "created_from":
                            case "createdfrom":
                            case "createdatdesde":
                            case "creado_desde":
                                if (DateTime.TryParse(valor, out var createdFrom))
                                    query = query.Where(x => x.CreatedAt.Date >= createdFrom.Date);
                                break;

                            case "createdatto":
                            case "created_to":
                            case "createdto":
                            case "createdathasta":
                            case "creado_hasta":
                                if (DateTime.TryParse(valor, out var createdTo))
                                    query = query.Where(x => x.CreatedAt.Date <= createdTo.Date);
                                break;

                            case "description":
                            case "descripcion":
                                query = query.Where(x => x.Description != null && x.Description.Contains(valor));
                                break;

                            default:
                                break;
                        }
                    }
                }

                // bùsqueda global opcional
                if (!string.IsNullOrWhiteSpace(request?.Buscar))
                {
                    var term = request.Buscar.Trim();
                    var isLong = long.TryParse(term, out var termLong);
                    var isDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out var termDecimal);
                    var isDate = DateTime.TryParse(term, out var termDate);

                    query = query.Where(x =>
                        (x.User != null && EF.Functions.Like(x.User, $"%{term}%")) ||
                        (x.Agente != null && EF.Functions.Like(x.Agente, $"%{term}%")) ||
                        (x.IP != null && EF.Functions.Like(x.IP, $"%{term}%")) ||
                        (x.Action != null && EF.Functions.Like(x.Action, $"%{term}%")) ||
                        (x.Description != null && EF.Functions.Like(x.Description, $"%{term}%")) ||
                        (x.Transaction != null && EF.Functions.Like(x.Transaction, $"%{term}%")) ||
                        (isLong && (x.Id == termLong || x.RaffleId == termLong || x.TicketNumber == termLong || x.UserId == termLong || x.AgenteId == termLong)) ||
                        (isDecimal && x.Amount != null && x.Amount == termDecimal) ||
                        (isDate && x.Date.Date   >= termDate.Date && x.Date.Date < termDate.AddDays(1).Date) ||
                        (isDate && x.CreatedAt.Date >= termDate.Date && x.CreatedAt.Date < termDate.AddDays(1).Date)
                    );
                }

                var totalElementos = await query.CountAsync();

                var lista = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarTransactionsResponse
                {
                    EsExitoso = true,
                    TotalElementos = totalElementos,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    TotalPaginas = (int)Math.Ceiling((double)totalElementos / request.RegistrosPorPagina.Value),
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Datos = lista.ToDtoList()
                };
            }
            catch (Exception ex)
            {
                return new ListarTransactionsResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Mensaje = $"Error al listar las transacciones: {ex.Message}",
                    CodigoError = "LISTAR_TRANSACTIONS_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}