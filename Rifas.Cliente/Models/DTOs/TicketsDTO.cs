using Rifas.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class TicketsDTO
    {
        public long Id { get; set; }

        public long RaffleId { get; set; }

        public long UserId { get; set; }

        public long TicketNumber { get; set; }

        public DateTime? BuyedDate { get; set; }

        public TicketStatusEnum Status { get; set; }

        [StringLength(50)]
        public string StatusDescription { get; set; } = null!;

        public DateTime StatusDate { get; set; }
        /// <summary>
        /// fecha de creacion del registro
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
