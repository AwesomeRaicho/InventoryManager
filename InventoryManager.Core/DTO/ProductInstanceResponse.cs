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
        public string? status { get; set; }
        public byte[]? ConcurrencyStamp { get; set; }

    }
}
