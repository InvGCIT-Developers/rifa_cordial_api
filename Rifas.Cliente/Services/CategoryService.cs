using GCIT.Core.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rifas.Client.Interfaces;
using Rifas.Client.Mappers;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rifas.Client.Services
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<CrearCategoryResponse> CrearCategoriaAsync(CrearCategoryRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new CrearCategoryResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "CREAR_CATEGORY_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var entity = request.Datos.ToEntity();
                // set defaults
                entity.IsActive = true;

                await _categoryRepository.AddAsync(entity);
                await _categoryRepository.SaveChangesAsync();

                return new CrearCategoryResponse
                {
                    EsExitoso = true,
                    Mensaje = "Categoria creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando categoria");
                return new CrearCategoryResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear la categoria: {ex.Message}",
                    CodigoError = "CREAR_CATEGORY_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<ActualizarCategoryResponse> ActualizarCategoriaAsync(ActualizarCategoryRequest request)
        {
            try
            {
                if (request?.Datos == null)
                {
                    return new ActualizarCategoryResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request.Data es nulo",
                        CodigoError = "ACTUALIZAR_CATEGORY_INVALID_REQUEST",
                        Datos = null
                    };
                }

                var existing = await _categoryRepository.GetByIdAsync(request.Datos.Id);
                if (existing == null)
                {
                    return new ActualizarCategoryResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Categoria no encontrada",
                        CodigoError = "ACTUALIZAR_CATEGORY_NOT_FOUND",
                        Datos = null
                    };
                }

                // actualizar la instancia ya trackeada para evitar conflicto de seguimiento
                existing.Name = request.Datos.Name;
                existing.Description = request.Datos.Description;
                existing.IsActive = request.Datos.IsActive;

                await _categoryRepository.UpdateAsync(existing);
                await _categoryRepository.SaveChangesAsync();

                return new ActualizarCategoryResponse
                {
                    EsExitoso = true,
                    Mensaje = "Categoria actualizada correctamente",
                    Datos = existing.ToDto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando categoria");
                return new ActualizarCategoryResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al actualizar la categoria: {ex.Message}",
                    CodigoError = "ACTUALIZAR_CATEGORY_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }

        public async Task<BaseResponse> EliminarCategoriaAsync(long id)
        {
            try
            {
                var existing = await _categoryRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    return new BaseResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Categoria no encontrada",
                        CodigoError = "ELIMINAR_CATEGORY_NOT_FOUND"
                    };
                }

                existing.IsActive = false;
                await _categoryRepository.UpdateAsync(existing);
                await _categoryRepository.SaveChangesAsync();

                return new BaseResponse
                {
                    EsExitoso = true,
                    Mensaje = "Categoria eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando categoria");
                return new BaseResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al eliminar la categoria: {ex.Message}",
                    CodigoError = "ELIMINAR_CATEGORY_ERROR",
                    Errores = new[] { ex.Message }.ToList()
                };
            }
        }

        public async Task<ListarCategoryResponse> ListarCategoriasActivasAsync(ListarCategoryRequest request)
        {
            try
            {
                var query = _categoryRepository.AllNoTracking().Where(x => x.IsActive);
                var totalElementos = await query.CountAsync();
                var lista = await query
                    .OrderByDescending(x => x.Id)                    
                    .ToListAsync();

                return new ListarCategoryResponse
                {
                    EsExitoso = true,
                    TotalElementos = totalElementos,
                    TamanoPagina = request.RegistrosPorPagina.Value,
                    TotalPaginas = (int)Math.Ceiling((double)totalElementos / request.RegistrosPorPagina.Value),
                    Pagina = request.Pagina.Value,
                    Datos = lista.ToDtoList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando categorias activas");
                return new ListarCategoryResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request?.RegistrosPorPagina ?? 0,
                    Pagina = request?.Pagina ?? 0,
                    Mensaje = $"Error al listar las categorias activas: {ex.Message}",
                    CodigoError = "LISTAR_ACTIVE_CATEGORY_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }

        }

        public async Task<ListarCategoryResponse> ListarCategoriasAsync(ListarCategoryRequest request)
        {
            try
            {
                var query = _categoryRepository.AllNoTracking();

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
                            case "categoryid":
                            case "category_id":
                                if (long.TryParse(valor, out var id))
                                    query = query.Where(x => x.Id == id);
                                break;

                            case "description":
                            case "desc":
                            case "descripcion":
                                query = query.Where(x => x.Description.Contains(valor));
                                break;

                            case "isactive":
                            case "activo":
                                if (bool.TryParse(valor, out var isActive))
                                    query = query.Where(x => x.IsActive == isActive);
                                break;

                            default:
                                break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(request?.Buscar))
                {
                    var term = request.Buscar.Trim();
                    var isLong = long.TryParse(term, out var termLong);

                    query = query.Where(x =>
                        (x.Description != null && x.Description.Contains(term)) ||
                        (isLong && x.Id == termLong)
                    );
                }

                var totalElementos = await query.CountAsync();

                var lista = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                return new ListarCategoryResponse
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
                _logger.LogError(ex, "Error listando categorias");
                return new ListarCategoryResponse
                {
                    EsExitoso = false,
                    TotalElementos = 0,
                    TamanoPagina = request?.RegistrosPorPagina ?? 0,
                    Pagina = request?.Pagina ?? 0,
                    FiltrosAplicados = request?.Filtros,
                    OrdenarPor = "Id",
                    Orden = "DESC",
                    Mensaje = $"Error al listar las categorias: {ex.Message}",
                    CodigoError = "LISTAR_CATEGORY_ERROR",
                    Errores = new[] { ex.Message }.ToList(),
                    Datos = null
                };
            }
        }
    }
}
