using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? ProductNumber {  get; set; } 
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int StockAmount { get; set; } = 0;
        public byte[]? ConcurrencyStamp { get; set; }

        // Foreign Keys
        public Guid? ProductTypeId { get; set; }

        //Navigation
        public ProductType? ProductType { get; set;}
    }
}
