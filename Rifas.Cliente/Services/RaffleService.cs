using GCIT.Core.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Common;
using Rifas.Client.Interfaces;
using Rifas.Client.Mappers;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Models.DTOs.Request;
using Rifas.Client.Models.DTOs.Response;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Services.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Rifas.Client.Modulos.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly IRaffleRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ITicketsRepository _ticketsRepository;
        private readonly ICloudflareService _cloudflareService;

        public RaffleService(IRaffleRepository repository, ICategoryRepository categoryRepository, IPurchaseRepository purchaseRepository, ITicketsRepository ticketsRepository, ICloudflareService cloudflareService)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _purchaseRepository = purchaseRepository;
            _ticketsRepository = ticketsRepository;
            _cloudflareService = cloudflareService;
        }


        public async Task<VerificarTicketNumberResponse> DisponibleTicketNumberAsync(VerificarTicketNumberRequest request)
        {
            // Implementación usando JOIN entre Raffles, Purchases y Tickets
            // SELECT * FROM Raffles r
            // INNER JOIN Purchases p ON r.Id = p.RaffleId
            // INNER JOIN Tickets t ON r.Id = t.RaffleId AND p.Id = t.PurchaseId
            // WHERE r.Id = @RaffleId AND t.TicketNumber = @TicketNumber

            if (request == null) return new VerificarTicketNumberResponse { Datos = false, EsExitoso = true, Mensaje = "OK" };

            var raffle = await _repository.GetByIdAsync(request.RaffleId);

            if (raffle == null || !raffle.IsActive)
            {
                return new VerificarTicketNumberResponse { Datos = true, EsExitoso = true, Mensaje = "OK" };
            }

            if(raffle.EndAt != null && raffle.EndAt <= DateTime.UtcNow)
            {
                return new VerificarTicketNumberResponse { Datos = true, EsExitoso = true, Mensaje = "OK" };
            }

            if (raffle.TopNUmber != null && request.TicketNumber > raffle.TopNUmber)
            {
                return new VerificarTicketNumberResponse { Datos = true, EsExitoso = true, Mensaje = "OK" };
            }
            else if (raffle.TopNUmber == null && request.TicketNumber > (Math.Pow(10, raffle.level) - 1))
            {
                return new VerificarTicketNumberResponse { Datos = true, EsExitoso = true, Mensaje = "OK" };
            }

            // Construir consulta LINQ con joins
            var q = from r in _repository.AllNoTracking()
                    join p in _purchaseRepository.AllNoTracking() on r.Id equals p.RaffleId
                    join t in _ticketsRepository.AllNoTracking() on p.Id equals t.PurchaseId
                    where r.Id == request.RaffleId  
                    select new { r, p, t };

            
                q = q.Where(x => x.t.TicketNumber == request.TicketNumber);
            

            var exists = await q.AnyAsync();
            return new VerificarTicketNumberResponse { Datos = exists, EsExitoso = true, Mensaje = "OK" };
        }

        public async Task<CrearRaffleResponse> CrearConImagenFromFormAsync(CrearRaffleWithFileRequest form)
        {
            try
            {
                if (form == null)
                {
                    return new CrearRaffleResponse
                    {
                        EsExitoso = false,
                        Mensaje = "Request es nulo",
                        CodigoError = "CREAR_RAFFLE_INVALID_REQUEST",
                        Datos = null
                    };
                }

                // Mapear campos del formulario a la entidad
                var entity = new Entities.RaffleEntity
                {
                    RaffleNumber = form.RaffleNumber,
                    level = form.level,
                    TopNUmber = form.TopNUmber,
                    BottomNumber = form.BottomNumber,
                    GarantedWinner = form.GarantedWinner,
                    AmountActive = form.AmountActive,
                    ImageUrl = null,
                    ImageFile = null, // se llenará después de subir la imagen
                    Title = form.Title,
                    Description = form.Description,
                    Sold = form.Sold,
                    Total = form.Total,
                    Price = form.Price,
                    TotalTickets = form.TotalTickets,
                    Participants = form.Participants,
                    Organizer = form.Organizer,
                    OrganizerRating = form.OrganizerRating,
                    OrganizerRatingCount = form.OrganizerRatingCount,
                    Category = form.Category,
                    IsActive = form.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    StartedAt = DateTime.UtcNow,
                    EndAt = form.EndAt  
                };

                // Persistir rifa inicialmente
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                // Si vino archivo, intentar subir a Cloudflare
                if (form.File != null && form.File.Length > 0)
                {
                    try
                    {
                        using var ms = new MemoryStream();
                        await form.File.CopyToAsync(ms);
                        var bytes = ms.ToArray();

                        // la implementación de UploadImageAsync acepta (byte[] fileBytes, string fileName, long raffleId)
                        var uploadResp = await _cloudflareService.UploadImageAsync(bytes, form.File.FileName, entity.Id);

                        if (uploadResp != null && uploadResp.result != null)
                        {
                            // intentar obtener first variant o filename
                            string? imageUrl = null;

                            if (uploadResp.result.variants != null && uploadResp.result.variants.Count > 0)
                            {
                                imageUrl = uploadResp.result.variants[0];
                            }
                            else if (!string.IsNullOrWhiteSpace(uploadResp.result.filename))
                            {
                                imageUrl = uploadResp.result.filename;
                            }

                            if (!string.IsNullOrWhiteSpace(imageUrl))
                            {
                                entity.ImageUrl = imageUrl;
                                entity.ImageFile = form.File.FileName;
                                await _repository.UpdateAsync(entity);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception exUpload)
                    {
                        // Si la subida falla, eliminamos la rifa creada para mantener consistencia
                        try
                        {
                            await _repository.DeleteAsync(entity);
                            await _repository.SaveChangesAsync();
                        }
                        catch
                        {
                            // ignorar errores al intentar limpiar, pero informar al cliente
                        }

                        return new CrearRaffleResponse
                        {
                            EsExitoso = false,
                            Mensaje = $"Error al subir la imagen: {exUpload.Message}",
                            CodigoError = "UPLOAD_IMAGE_ERROR",
                            Datos = null
                        };
                    }
                }

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

                existing.IsActive = false;
                await _repository.UpdateAsync(existing);
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
                // incluir CategoryEntity para que la navegación esté poblada
                var entity = await _repository.AllNoTracking().Include(x => x.CategoryEntity).FirstOrDefaultAsync(x => x.Id == id);
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
                // consulta base
                var query = _repository.AllNoTracking();

                int? categoryFilterId = null;

                // aplicar filtros opcionales si vienen en request.Filtros (misma lógica que en TicketsService)
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
                            case "raffleid":
                            case "raffle_id":
                                if (long.TryParse(valor, out var id))
                                {
                                    query = query.Where(x => x.Id == id);
                                }
                                break;

                            case "title":
                            case "nombre":
                            case "rafflename":
                                query = query.Where(x => x.Title.Contains(valor));
                                break;

                            case "category":
                            case "categoria":
                                if (int.TryParse(valor, out var catId))
                                {
                                    categoryFilterId = catId;
                                    query = query.Where(x => x.Category == catId);
                                }
                                break;

                            case "isactive":
                            case "activo":
                                if (bool.TryParse(valor, out var isActive))
                                {
                                    query = query.Where(x => x.IsActive == isActive);
                                }
                                break;

                            case "createdatfrom":
                            case "created_from":
                            case "createdfrom":
                            case "createdatdesde":
                            case "creado_desde":
                                if (DateTime.TryParse(valor, out var createdFrom))
                                {
                                    query = query.Where(x => x.CreatedAt >= createdFrom);
                                }
                                break;

                            case "createdatto":
                            case "created_to":
                            case "createdto":
                            case "createdathasta":
                            case "creado_hasta":
                                if (DateTime.TryParse(valor, out var createdTo))
                                {
                                    query = query.Where(x => x.CreatedAt <= createdTo);
                                }
                                break;

                            case "endatfrom":
                            case "end_from":
                            case "endfrom":
                                if (DateTime.TryParse(valor, out var endFrom))
                                {
                                    query = query.Where(x => x.EndAt != null && x.EndAt >= endFrom);
                                }
                                break;

                            case "endatto":
                            case "end_to":
                            case "endto":
                                if (DateTime.TryParse(valor, out var endTo))
                                {
                                    query = query.Where(x => x.EndAt != null && x.EndAt <= endTo);
                                }
                                break;

                            case "organizer":
                            case "organizador":
                                query = query.Where(x => x.Organizer.Contains(valor));
                                break;

                            case "pricefrom":
                            case "price_from":
                            case "precio_desde":
                                if (decimal.TryParse(valor, out var pFrom))
                                {
                                    query = query.Where(x => x.Price != null && x.Price >= pFrom);
                                }
                                break;

                            case "priceto":
                            case "price_to":
                            case "precio_hasta":
                                if (decimal.TryParse(valor, out var pTo))
                                {
                                    query = query.Where(x => x.Price != null && x.Price <= pTo);
                                }
                                break;

                            default:
                                // ignorar filtros desconocidos
                                break;
                        }
                    }
                }

                // búsqueda global en todos los campos relevantes si viene request.Buscar
                if (!string.IsNullOrWhiteSpace(request?.Buscar))
                {
                    var term = request.Buscar.Trim();
                    var isLong = long.TryParse(term, out var termLong);
                    var isInt = int.TryParse(term, out var termInt);
                    var isDecimal = decimal.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out var termDecimal);
                    var isDate = DateTime.TryParse(term, out var termDate);

                    query = query.Where(x =>
                        // campos string
                        (x.Title != null && EF.Functions.Like(x.Title, $"%{term}%")) ||
                        (x.Description != null && EF.Functions.Like(x.Description, $"%{term}%")) ||
                        (x.ImageUrl != null && EF.Functions.Like(x.ImageUrl, $"%{term}%")) ||
                        (x.Organizer != null && EF.Functions.Like(x.Organizer, $"%{term}%")) ||

                        // búsqueda por enum nombre cuando el término no es numérico
                        (!isInt && x.Category.ToString().Contains(term)) ||

                        // comparaciones numéricas cuando el término es numérico
                        (isLong && x.Id == termLong) ||
                        (isInt && x.TotalTickets == termInt) ||
                        (isInt && x.Participants == termInt) ||

                        // precio comparado si decimal
                        (isDecimal && x.Price != null && x.Price == termDecimal) ||

                        // búsquedas por fecha aproximada (día)
                        (isDate && x.CreatedAt >= termDate && x.CreatedAt < termDate.AddDays(1)) ||
                        (isDate && x.EndAt != null && x.EndAt >= termDate && x.EndAt < termDate.AddDays(1))
                    );
                }

                // contar después de aplicar filtros
                var totalElementos = await query.CountAsync();

                // paginación y ordenación
                var lista = await query
                    .Include(x => x.CategoryEntity)
                    .OrderByDescending(x => x.Id)
                    .Skip((request.Pagina.Value - 1) * request.RegistrosPorPagina.Value)
                    .Take(request.RegistrosPorPagina.Value)
                    .ToListAsync();

                var response = new ListarRaffleResponse
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

                return response;
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