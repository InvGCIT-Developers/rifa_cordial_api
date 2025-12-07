
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Models.DTOs.Request
{
    public class AuthRequest
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string IP { get; set; } = null!;
        public string WebSite { get; set; } = null!;
    }
}