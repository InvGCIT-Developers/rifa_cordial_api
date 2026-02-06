using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rifas.Client.Entities
{
    public class TicketsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long RaffleId { get; set; }

        public long? PurchaseId { get; set; }  // nullable si reservas antes de confirmar
        public long UserId { get; set; }

        public long TicketNumber { get; set; }

        public DateTime? BuyedDate { get; set; }

        public TicketStatusEnum Status { get; set; }

        public TicketStateEnum State { get; set; } = TicketStateEnum.SinResultado;

        [StringLength(50)]
        public string StatusDescription { get; set; } = string.Empty;

        public DateTime StatusDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual PurchaseEntity? Purchase { get; set; }

        public virtual RaffleEntity? Raffle { get; set; }


    }
}
