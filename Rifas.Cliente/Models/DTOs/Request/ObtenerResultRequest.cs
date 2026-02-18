using GCIT.Core.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs.Request
{
    public class ObtenerResultRequest: BaseRequest
    {
        public int positionWin { get; set; }
        public long raffleId { get; set; }
    }
}
