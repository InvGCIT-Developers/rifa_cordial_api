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
                var lista = await _repository
                    .AllNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarPurchaseResponse
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