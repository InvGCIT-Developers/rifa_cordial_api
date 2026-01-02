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
                    Mensaje = "Transacción creada correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new CrearTransactionsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al crear la transacción: {ex.Message}",
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
                        Mensaje = "Transacción no encontrada",
                        CodigoError = "OBTENER_TRANSACTION_NOT_FOUND",
                        Datos = null
                    };
                }

                return new ObtenerTransactionsResponse
                {
                    EsExitoso = true,
                    Mensaje = "Transacción obtenida correctamente",
                    Datos = entity.ToDto()
                };
            }
            catch (Exception ex)
            {
                return new ObtenerTransactionsResponse
                {
                    EsExitoso = false,
                    Mensaje = $"Error al obtener la transacción: {ex.Message}",
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
                var lista = await _repository
                        .AllNoTracking()
                        .Take(request.RegistrosPorPagina.Value)
                        .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                        .ToListAsync();

                return new ListarTransactionsResponse
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