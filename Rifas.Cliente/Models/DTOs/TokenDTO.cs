
using System;

namespace Rifas.Client.Models.DTOs
{
    public class TokenDTO
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}