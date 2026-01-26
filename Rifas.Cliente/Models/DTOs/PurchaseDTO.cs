using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class PurchaseDTO
    {
        public long? Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long RaffleId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }

        public List<TicketsDTO>? Tickets { get; set; } = new List<TicketsDTO>();

        public bool IsActive { get; set; }
    }
}
