using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rifas.Client.Entities
{
    public class TransactionsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long? RaffleId { get; set; }
        public long? TicketNumber { get; set; }
        public long UserId { get; set; }

        [StringLength(50)]
        public string User { get; set; } = null!;

        public long AgenteId { get; set; }

        [StringLength(50)]
        public string Agente { get; set; } = null!;

        [StringLength(50)]
        public string IP { get; set; } = null!;
        public string? Transaction { get; set; }
        public DateTime Date { get; set; }
        public Decimal? PlayerBalance { get; set; }
        public Decimal? Amount { get; set; }

        [StringLength(50)]
        public string? Action { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? RestMethod { get; set; }

        [StringLength(4000)]
        public string? JsonRequest { get; set; }

        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
