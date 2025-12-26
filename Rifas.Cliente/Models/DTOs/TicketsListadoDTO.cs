using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class TicketsListadoDTO
    {
        public long RaffleId { get; set; }
        public string RaffleName { get; set; } = null!;
        public string RaffleImage { get; set; } = null!;
        public long TicketNumber { get; set; } 
        public string Note { get; set; } = null!;

        public RifaCategoriaEnum Category { get; set; }
        public TicketStatusEnum Status { get; set; }

        public DateTime? PurchasedAt { get; set; }
    }
}
