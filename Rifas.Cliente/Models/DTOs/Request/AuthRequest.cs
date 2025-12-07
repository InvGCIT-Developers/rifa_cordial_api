
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs.Request
{
    public class AuthRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}