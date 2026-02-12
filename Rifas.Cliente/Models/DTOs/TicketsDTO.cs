using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class TicketsDTO
    {
        public long? Id { get; set; }

        public long RaffleId { get; set; }

        public long? PurchaseId { get; set; }

        public long UserId { get; set; }

        public long TicketNumber { get; set; }

        public DateTime? BuyedDate { get; set; }

        public TicketStatusEnum Status { get; set; }

        public string TicketStatus { get => Status.GetDisplayName(); }

        public TicketStateEnum State { get; set; } = TicketStateEnum.SinResultado;

        public string TicketState { get => State.GetDisplayName(); }
        

        [StringLength(50)]
        public string StatusDescription { get; set; } = string.Empty;

        public DateTime StatusDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
