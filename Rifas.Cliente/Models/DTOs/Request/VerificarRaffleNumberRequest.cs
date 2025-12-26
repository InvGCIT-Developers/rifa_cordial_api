using GCIT.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs.Request
{
    public class VerificarRaffleNumberRequest: BaseRequest
    {
        public long RaffleId { get; set; }
        public string RaffleNumber { get; set; } = null!;
    }
}
