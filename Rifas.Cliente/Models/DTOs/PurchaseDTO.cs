using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class PurchaseDTO
    {
        public long? Id { get; set; }
        public long UserId { get; set; }
        public long RaffleId { get; set; }
       
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }

        public List<TicketsDTO>? Tickets { get; set; } = new List<TicketsDTO>();

        public bool IsActive { get; set; }
    }
}
