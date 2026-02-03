using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Rifas.Client.Models.DTOs.Request
{
    public class CrearRaffleWithFileRequest
    {
        public IFormFile? File { get; set; }

        [Required]
        public string RaffleNumber { get; set; } = string.Empty;

        [Required]
        public int Level { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomNumber { get; set; }

        /// <summary>
        /// garantizar el resultado ganador
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? GarantedWinner { get; set; }

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

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Sold { get; set; }

        [Required]
        public int Total { get; set; }

        [Required]
        public decimal Price { get; set; }

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

        [Required]
        public int OrganizerRatingCount { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? EndAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? StartedAt { get; set; }
    }
}
