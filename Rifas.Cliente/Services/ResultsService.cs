using GCIT.Core.Models.Base;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Interfaces;
using Rifas.Client.Mappers;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

                existing.IsActive = false;
                await _repository.UpdateAsync(existing);
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
                var query = _repository.AllNoTracking()
                    .Include(r => r.Ticket)
                    .Include(r => r.Raffle).AsQueryable();

                // aplicar filtros opcionales
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
                            case "id":
                            case "resultid":
                            case "result_id":
                                if (long.TryParse(valor, out var id))
                                    query = query.Where(x => x.Id == id);
                                break;

                            case "raffleid":
                            case "raffle_id":
                                if (long.TryParse(valor, out var rId))
                                    query = query.Where(x => x.RaffleId == rId);
                                break;

                            case "rafflenumber":
                            case "raffle_number":
                                query = query.Where(x => EF.Functions.Like(x.RaffleNumber, $"%{valor}%"));
                                break;

                            case "winningnumber":
                            case "winning_number":
                                query = query.Where(x => EF.Functions.Like(x.WinningNumber, $"%{valor}%"));
                                break;

                            case "isactive":
                            case "activo":
                                if (bool.TryParse(valor, out var isActive))
                                    query = query.Where(x => x.IsActive == isActive);
                                break;

                            case "lotterydatefrom":
                            case "lottery_from":
                                if (DateTime.TryParse(valor, out var fromDt))
                                    query = query.Where(x => x.LotteryDate >= fromDt);
                                break;

                            case "lotterydateto":
                            case "lottery_to":
                                if (DateTime.TryParse(valor, out var toDt))
                                    query = query.Where(x => x.LotteryDate <= toDt);
                                break;

                            default:
                                break;
                        }
                    }
                }

                // búsqueda global
                if (!string.IsNullOrWhiteSpace(request?.Buscar))
                {
                    var term = request.Buscar.Trim();
                    var isLong = long.TryParse(term, out var termLong);
                    var isInt = int.TryParse(term, out var termInt);
                    var isDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out var termDecimal);
                    var isDate = DateTime.TryParse(term, out var termDate);

                    query = query.Where(x =>
                        (x.RaffleNumber != null && EF.Functions.Like(x.RaffleNumber, $"%{term}%")) ||
                        (x.WinningNumber != null && EF.Functions.Like(x.WinningNumber, $"%{term}%")) ||
                        (x.FirstPlace != null && EF.Functions.Like(x.FirstPlace, $"%{term}%")) ||
                        (x.SecondPlace != null && EF.Functions.Like(x.SecondPlace, $"%{term}%")) ||
                        (x.ThirdPlace != null && EF.Functions.Like(x.ThirdPlace, $"%{term}%")) ||
                        (isLong && x.Id == termLong) ||
                        (isLong && x.RaffleId == termLong) ||
                        (isDate && x.LotteryDate >= termDate && x.LotteryDate < termDate.AddDays(1)) ||
                        (!isDecimal && x.IsActive.ToString().Contains(term))
                    );
                }

                var totalElementos = await query.CountAsync();

                var lista = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarResultsResponse
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