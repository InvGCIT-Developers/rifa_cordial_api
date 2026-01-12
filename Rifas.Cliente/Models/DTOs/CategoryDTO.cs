using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rifas.Client.Models.DTOs
{
    public class CategoryDTO
    {
        public long? Id { get; set; }

        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
