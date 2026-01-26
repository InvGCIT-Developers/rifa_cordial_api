using GCIT.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs.Request
{
    public class VerificarTicketNumberRequest: BaseRequest
    {
        public long RaffleId { get; set; }
        public long TicketNumber { get; set; }
    }
}
