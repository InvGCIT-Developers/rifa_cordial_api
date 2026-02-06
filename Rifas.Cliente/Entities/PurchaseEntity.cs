using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rifas.Client.Entities
{
    public class PurchaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RaffleId { get; set; }

        [StringLength(6)]
        public string RaffleNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; }

        public virtual ICollection<TicketsEntity> Tickets { get; set; } = new List<TicketsEntity>();

        public virtual RaffleEntity? Raffle { get; set; }
    }
}
