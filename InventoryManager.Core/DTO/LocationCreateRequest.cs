using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class LocationCreateRequest
    {
        [Required]
        [MaxLength(22)]
        public string? Name { get; set; }

    }
}
