using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Rifas.Client.Models.DTOs
{
    public class RaffleDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id { get; set; }

        [StringLength(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RaffleNumber { get; set; } 

        [Required]
        public int level { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopNUmber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomNumber { get; set; }

        /// <summary>
        /// garantizar el resultado ganador
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? GarantedWinner { get; set; }


        [Required]
        [Range(1, 3)]
        public int WinnersNumber { get; set; } = 1;

        /// <summary>
        /// monto del activo (ej. 1500.00)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? AmountActive { get; set; }

        /// <summary>
        /// ruta de la imagen
        /// </summary>
        [StringLength(250)]        
        public string? ImageUrl { get; set; } 

        [StringLength(250)]
        public string? ImageFile { get; set; }

        [StringLength(50)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        ///  cantidad vendida (ej. 720)
        /// </summary>
        [Required]
        public int Sold { get; set; }

        /// <summary>
        /// total de entradas (ej. 1000)
        /// </summary>
        [Required]
        public int Total { get; set; }

        /// <summary>
        /// precio mostrado (ej. "$5" o 5)
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// úmero de boletos (opcional)
        /// </summary>
        [Required]
        public int TotalTickets { get; set; }

        /// <summary>
        /// número de participantes
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Participants { get; set; }

        /// <summary>
        /// nombre del organizador (opcional)
        /// </summary>
        [StringLength(100)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Organizer { get; set; }

        /// <summary>
        /// Gets or sets the organizer's rating for the event.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OrganizerRating { get; set; }

        /// <summary>
        /// número de valoraciones
        /// </summary>
        [Required]
        public int OrganizerRatingCount { get; set; }

        /// <summary>
        /// categoría de la rifa; ahora se incluye como DTO
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CategoryDTO? Category { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<TicketsDTO>? Tickets { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PurchaseDTO>? Purchases { get; set; }

        /// <summary>
        /// indica si la rifa está activa
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// fecha de finalización (opcional)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? EndAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? StartedAt { get; set; }
    }
}
