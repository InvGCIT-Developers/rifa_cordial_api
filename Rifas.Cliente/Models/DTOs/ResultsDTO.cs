using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class ResultsDTO
    {
        public long? Id { get; set; }
        public long RaffleId { get; set; }

        [StringLength(6)]
        public string RaffleNumber { get; set; } = null!;

        public long? TicketId { get; set; }        

        [StringLength(6)]
        public string WinningNumber { get; set; } = null!;

        [StringLength(6)]
        public string? FirstPlace { get; set; }

        [StringLength(6)]
        public string? SecondPlace { get; set; }

        [StringLength(6)]
        public string? ThirdPlace { get; set; }

        public bool IsActive { get; set; }
        public DateTime LotteryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public TicketsDTO? Ticket { get; set; }
        //public TicketsListadoDTO? Ticket { get; set; }

        public RaffleDTO? Raffle { get; set; }

    }
}
