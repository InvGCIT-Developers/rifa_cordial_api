using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class TicketsListadoDTO
    {
        public long RaffleId { get; set; }
        public long? PurchaseId { get; set; }
        public string RaffleName { get; set; } = null!;
        public string RaffleImage { get; set; } = null!;
        public long TicketNumber { get; set; }
        public string Note { get; set; } = null!;

        public long Category { get; set; }
        public TicketStatusEnum Status { get; set; }
        public TicketStateEnum State { get; set; } = TicketStateEnum.SinResultado;
        public string StateDescription
        {
            get
            {
                return State.GetDisplayName();

            }
        }
        public DateTime? PurchasedAt { get; set; }

        public PurchaseDTO? Purchase { get; set; }
    }
}
