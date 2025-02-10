using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductPutRequest
    {
        [Required]
        public string? Id { get; set; }
        public string? ProductNumber { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        [Required]
        public byte[]? ConcurrencyStamp { get; set; }

        // Foreign Keys
        public string? ProductTypeId { get; set; }
    }
}
