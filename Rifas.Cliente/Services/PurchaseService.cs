using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
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
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repository;

        public PurchaseService(IPurchaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<CrearPurchaseResponse> CrearAsync(CrearPurchaseRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new CrearPurchaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_PURCHASE_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                entity.PurchaseDate = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return new CrearPurchaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Purchase creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearPurchaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear la purchase: {ex.Message}",
                    CodigoError = "CREAR_PURCHASE_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ActualizarPurchaseResponse> ActualizarAsync(ActualizarPurchaseRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new ActualizarPurchaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "ACTUALIZAR_PURCHASE_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var existing = await _repository.GetByIdAsync(request.Datos.Id);
                if (existing == null)
                {
                    return new ActualizarPurchaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Purchase no encontrada",
                        CodigoError = "ACTUALIZAR_PURCHASE_NOT_FOUND",
                        Datos = null
                    };
                }

                var toUpdate = request.Datos.ToEntity();
                toUpdate.PurchaseDate = existing.PurchaseDate;

                await _repository.UpdateAsync(toUpdate);
                await _repository.SaveChangesAsync();

                return new ActualizarPurchaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Purchase actualizada correctamente",
                    Datos = toUpdate.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ActualizarPurchaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al actualizar la purchase: {ex.Message}",
                    CodigoError = "ACTUALIZAR_PURCHASE_ERROR",
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
                        Mensaje = "Purchase no encontrada",
                        CodigoError = "ELIMINAR_PURCHASE_NOT_FOUND"
                    };
                }

                await _repository.DeleteAsync(existing);
                await _repository.SaveChangesAsync();

                return new BaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Purchase eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al eliminar la purchase: {ex.Message}",
                    CodigoError = "ELIMINAR_PURCHASE_ERROR",
                    Errores = new[] { ex.Message }.ToList()
                };
            }
        }

        public async Task<ObtenerPurchaseResponse> ObtenerPorIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ObtenerPurchaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Purchase no encontrada",
                        CodigoError = "OBTENER_PURCHASE_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerPurchaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Purchase obtenida correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerPurchaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener la purchase: {ex.Message}",
                    CodigoError = "OBTENER_PURCHASE_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ListarPurchaseResponse> ListarAsync(ListarPurchaseRequest request)
        {
            try
            {
                var query = _repository.AllNoTracking();

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
                            case "purchaseid":
                            case "purchase_id":
                                if (long.TryParse(valor, out var id))
                                    query = query.Where(x => x.Id == id);
                                break;

                            case "raffleid":
                            case "raffle_id":
                                if (long.TryParse(valor, out var rId))
                                    query = query.Where(x => x.RaffleId == rId);
                                break;

                            case "userid":
                            case "user_id":
                            case "user":
                                if (long.TryParse(valor, out var uId))
                                    query = query.Where(x => x.UserId == uId);
                                break;

                            case "rafflenumber":
                            case "raffle_number":
                            case "number":
                                query = query.Where(x => EF.Functions.Like(x.RaffleNumber, $"%{valor}%"));
                                break;

                            case "isactive":
                            case "activo":
                                if (bool.TryParse(valor, out var isActive))
                                    query = query.Where(x => x.IsActive == isActive);
                                break;

                            case "purchasedatefrom":
                            case "purchase_from":
                                if (DateTime.TryParse(valor, out var fromDt))
                                    query = query.Where(x => x.PurchaseDate >= fromDt);
                                break;

                            case "purchasedateto":
                            case "purchase_to":
                                if (DateTime.TryParse(valor, out var toDt))
                                    query = query.Where(x => x.PurchaseDate <= toDt);
                                break;

                            case "totalamountfrom":
                            case "total_from":
                            case "precio_desde":
                                if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var tFrom))
                                    query = query.Where(x => x.TotalAmount >= tFrom);
                                break;

                            case "totalamountto":
                            case "total_to":
                            case "precio_hasta":
                                if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var tTo))
                                    query = query.Where(x => x.TotalAmount <= tTo);
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
                        (isLong && x.Id == termLong) ||
                        (isLong && x.RaffleId == termLong) ||
                        (isLong && x.UserId == termLong) ||
                        (isInt && x.Quantity == termInt) ||
                        (isDecimal && x.TotalAmount == termDecimal) ||
                        (isDate && x.PurchaseDate >= termDate && x.PurchaseDate < termDate.AddDays(1)) ||
                        (!isDecimal && x.IsActive.ToString().Contains(term))
                    );
                }

                var totalElementos = await query.CountAsync();

                var lista = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarPurchaseResponse
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
                return new ListarPurchaseResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    Pagina = request.Pagina.Value,
                    FiltrosAplicados = request.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Mensaje = $"Error al listar las purchases: {ex.Message}",
                    CodigoError = "LISTAR_PURCHASES_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}