using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.DTO
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductNumber { get; set;}
        public decimal? Price { get; set; }
        public int StockAmount { get; set; } = 0;
        public byte[]? ConcurrencyStamp { get; set; }
        public string? ProductType { get; set; }
        public string? ProductTypeId {  get; set; }
    }
}
