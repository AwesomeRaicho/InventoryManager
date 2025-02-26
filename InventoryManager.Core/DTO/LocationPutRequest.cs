using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class LocationPutRequest
    {
        [Required]
        public string? Id { get; set; }
        [Required]
        [MaxLength(22)]
        public string? Name { get; set; }
        [Required]
        public byte[]? ConcurrencyStamp { get; set; }
    }
}
