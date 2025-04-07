using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductInstanceResponse
    {
        public string? Id { get; set; }
        public string? Barcode { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Status { get; set; }
        public decimal? PurchasePrice { get; set; }


        public DateTime? EntryDate { get; set; }

    }
}
