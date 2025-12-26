using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class PurchaseDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RaffleId { get; set; }

        [StringLength(6)]
        public string RaffleNumber { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }

        public bool IsActive { get; set; }
    }
}
