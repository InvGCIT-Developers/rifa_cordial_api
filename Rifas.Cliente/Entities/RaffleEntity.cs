using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rifas.Client.Entities
{
    public class RaffleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(6)]
        public string RaffleNumber { get; set; } = null!;

        [Required]
        public int level { get; set; }

        public int? TopNUmber { get; set; }

        public int? BottomNumber { get; set; }

        /// <summary>
        /// garantizar el resultado ganador
        /// </summary>
        public bool? GarantedWinner { get; set; }

        /// <summary>
        /// monto del activo (ej. 1500.00)
        /// </summary>
        public decimal? AmountActive { get; set; }

        /// <summary>
        /// ruta de la imagen
        /// </summary>
        [StringLength(250)]
        public string? ImageUrl { get; set; }

        [StringLength(250)]
        public string? ImageFile { get; set; } 


        [StringLength(50)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;

        /// <summary>
        ///  cantidad vendida (ej. 720)
        /// </summary>
        public int Sold { get; set; }

        /// <summary>
        /// total de entradas (ej. 1000)
        /// </summary>
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
        public int? Participants { get; set; }

        /// <summary>
        /// nombre del organizador (opcional)
        /// </summary>
        [StringLength(100)]
        public string? Organizer { get; set; }

        /// <summary>
        /// Gets or sets the organizer's rating for the event.
        /// </summary>
        public int? OrganizerRating { get; set; }

        /// <summary>
        /// número de valoraciones
        /// </summary>
        public int OrganizerRatingCount { get; set; }

        /// <summary>
        /// categoría de la rifa; almacena el Id de CategoryEntity
        /// </summary>
        public long Category { get; set; }

        /// <summary>
        /// navegación a la categoría
        /// </summary>
        public virtual CategoryEntity? CategoryEntity { get; set; }

        /// <summary>
        /// indica si la rifa está activa
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// fecha de inicio (opcional)
        /// </summary>
        public DateTime? StartedAt { get; set; } 

        /// <summary>
        /// fecha de finalización (opcional)
        /// </summary>
        public DateTime? EndAt { get; set; }

        
    }
}
