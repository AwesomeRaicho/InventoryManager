using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Models
{
    public class ProductInstance
    {
        public Guid Id { get; set; }
        public string? Barcode { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? SellDate { get; set; }
        public string? Status { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }
        public string? SoldBy { get; set; }

        // Foreign Keys
        public Guid? LocationId { get; set; } // FK for Location
        public Guid? ProductId { get; set; }  // FK for Product

        // Navigation properties
        public Location? Location { get; set; }
        public Product? Product { get; set; }




    }
}
