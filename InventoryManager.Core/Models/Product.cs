using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Timestamp]
        public byte[] ConcurrencyStamp { get; set; } = new byte[0];

        // Foreign Keys
        public Guid? ProductTypeId { get; set; }

        //Navigation
        public ProductType? ProductType { get; set;}

        //many-to-many
        public List<Product_Property>? Product_Property { get; set; }
    }
}
