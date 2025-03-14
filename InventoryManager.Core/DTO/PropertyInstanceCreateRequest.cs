using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class PropertyInstanceCreateRequest
    {
        [Required]
        public string? Name { get; set; }
        
        [Required] 
        public string? PropertyTypeId { get; set; }
    }
}
