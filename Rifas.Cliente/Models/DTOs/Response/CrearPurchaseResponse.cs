using GCIT.Core.Models.Base;
using Rifas.Client.Models.DTOs;
using System.Text.Json.Serialization;

namespace Rifas.Client.Models.DTOs.Response
{
    public class CrearPurchaseResponse : BaseResponse<PurchaseDTO>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<TicketsDTO>? TicketsNoDisponibles { get; set; } 
    }
}