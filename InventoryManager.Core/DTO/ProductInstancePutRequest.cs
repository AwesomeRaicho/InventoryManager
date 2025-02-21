using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductInstancePutRequest
    {
        public string? Id { get; set; }
        public string? Barcode { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string? Status { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }
        public string? LocationId { get; set; } 
        public string? ProductId { get; set; }

        public List<string>? ProductInstance_Property { get; set; }

    }
}
